using Grpc.Core;
using Grpc.Net.Client;
using Optimizely.PIM.Data.V1;
using System.Security.Cryptography;

namespace PimApi;

public class GrpcClientService
{
    private readonly string PimUrl;
    private readonly string AppKey;
    private readonly string AppSecret;

    public GrpcClientService(string? pimUrlBase, string? appKey, string? appSecret)
    {
        if (pimUrlBase is null || appKey is null || appSecret is null)
        {
            throw new ArgumentNullException();
        }

        AppKey = appKey;
        AppSecret = appSecret;

        var odataIndex = pimUrlBase.LastIndexOf("odata/");
        PimUrl = odataIndex < 0 ? pimUrlBase : pimUrlBase.Substring(0, odataIndex);
    }

    public async Task<ImportProductResponse> Import(
        string importTemplate,
        string timeZone,
        IReadOnlyCollection<Product> products
    )
    {
        using var channel = GrpcChannel.ForAddress(PimUrl);
        var client = new DataIntegration.DataIntegrationClient(GrpcChannel.ForAddress(PimUrl));

        var metadata = GenerateMetadata("ImportProduct", AppKey, AppSecret);
        using var call = client.ImportProduct(metadata);

        var numberOfRecords = products.Count;
        var batchSize = 50;
        var numberOfBatches = (numberOfRecords + batchSize - 1) / batchSize;

        var importRequest = new ImportProductRequest
        {
            TimeZone = timeZone, // For example: "Asia/Bangkok"
            ImportTemplate = importTemplate,
        };
        for (var i = 0; i < numberOfBatches; i++)
        {
            importRequest.Products.Clear();
            importRequest.Products.AddRange(products.Skip(i * batchSize).Take(batchSize));
            await call.RequestStream.WriteAsync(importRequest);
        }

        await call.RequestStream.CompleteAsync();
        return await call.ResponseAsync;
    }

    public async Task<GetImportStatusResponse> GetImportStatus(string requestId)
    {
        using var channel = GrpcChannel.ForAddress(PimUrl);
        var client = new DataIntegration.DataIntegrationClient(GrpcChannel.ForAddress(PimUrl));

        Metadata metadata = GenerateMetadata("GetImportStatus", AppKey, AppSecret);
        return await client.GetImportStatusAsync(
            new GetImportStatusRequest { RequestId = requestId },
            metadata
        );
    }

    private static Metadata GenerateMetadata(
        string methodName,
        string turnstileKey,
        string turnstileSecret
    )
    {
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var nonce = Guid.NewGuid().ToString("N");

        var bodyHashBytes = MD5.Create().ComputeHash(Array.Empty<byte>());
        var hashBody = Convert.ToBase64String(bodyHashBytes);

        var method = "POST";
        var path = $"/Optimizely.PIM.Data.V1.DataIntegration/{methodName}";
        var message =
            $"{turnstileKey}{method.ToString().ToUpper()}{path}{timestamp}{nonce}{hashBody}";
        var messageBytes = Encoding.UTF8.GetBytes(message);

        var hmacAlgorithm = new HMACSHA256(Convert.FromBase64String(turnstileSecret));
        var signatureHash = hmacAlgorithm.ComputeHash(messageBytes);
        var signature = Convert.ToBase64String(signatureHash);

        return new Metadata
        {
            { "Authorization", $"epi-hmac {turnstileKey}:{timestamp}:{nonce}:{signature}" },
        };
    }
}
