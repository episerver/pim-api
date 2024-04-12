using System.Diagnostics.CodeAnalysis;
using PimApi.Extensions;
using PimApi.JsonSerialization;

namespace PimApi.ConsoleApp;

// Partial class is used to only highlight important code in this main file
[ExcludeFromCodeCoverage]
public partial class Program
{
    /// <summary>Stored in root folder with solution</summary>
    internal const string ConnectionInformationFilePath =
        "../../../../../ConnectionInformation.json";

    private static async Task Main(string[] args)
    {
        var jsonSerializer = UseNewtonsoftSerializer(args)
            ? new NewtonsoftJsonSerializer()
            : new SystemTextJsonSerialzer() as IJsonSerializer;
        var fullConnectionFilePath = GetConnectionFile(args);

        var isStop = false;
        while (!isStop)
        {
            Console.Clear();
            Console.WriteLine("*************************************");
            Console.WriteLine("* Select option                     *");
            Console.WriteLine("*   1. Fetching data                *");
            Console.WriteLine("*   2. Integrating data             *");
            Console.WriteLine("*   3. Exit                         *");
            Console.WriteLine("*************************************");
            var selectedValue = ReadValue("Please enter a number:", string.Empty);

            isStop = selectedValue is null || stopwords.Contains(selectedValue);
            if (isStop)
            {
                continue;
            }

            switch (selectedValue)
            {
                case "1":
                    // see README.md about obtaining credentials from support
                    // create a file from ConnectionInformation.Sample.json if one does not exist and set provided AppKey and AppSecret
                    // IMPORTANT: Not any HttpClient will work, one needs to be configured with the tokens provided which will set the appropriate HTTP Headers for API requests.
                    // If using just a normal HttpClient all requests will be unauthorized!
                    var apiClient = await CreateAuthorizedPimApiClientFromFile(
                        fullConnectionFilePath,
                        jsonSerializer
                    );

                    await Execute(apiClient, jsonSerializer, GetQueries());
                    break;
                case "2":
                    var connectionInformation = await GetConnectionInformation(
                        fullConnectionFilePath,
                        jsonSerializer
                    );
                    await ShowGrpcServices(
                        jsonSerializer,
                        connectionInformation?.PimUrlBase,
                        connectionInformation?.AppKey,
                        connectionInformation?.AppSecret
                    );
                    break;
                case "3":
                    return;
                default:
                    Console.WriteLine("Unknown option, please select available options!");
                    break;
            }
        }
    }

    /// <summary> Creates http client that sends proper headers for PIM API access</summary>
    private static async Task<HttpClient> CreateAuthorizedPimApiClientFromFile(
        string fullFilePath,
        IJsonSerializer jsonSerializer
    )
    {
        var connectionInformation = await GetConnectionInformation(fullFilePath, jsonSerializer);
        return new ApiHttpClientFactory(connectionInformation).Create();
    }

    // first argument is path to JSON file
    private static string GetConnectionFile(string[] args) =>
        args.Length > 0
            ? args[0]
            : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConnectionInformationFilePath);

    // 2nd argument is serializer switch
    private static bool UseNewtonsoftSerializer(string[] args) =>
        args.Length > 1
        && bool.TryParse(args[1], out var useNewtonsoftSerializer)
        && useNewtonsoftSerializer;

    private static async Task<ConnectionInformation> GetConnectionInformation(
        string fullFilePath,
        IJsonSerializer jsonSerializer
    )
    {
        var connectionFile = new FileInfo(fullFilePath);

        // uses an extension method to convert FileInfo into ConnectionInformation class
        return await connectionFile.GetConnectionInformation(jsonSerializer);
    }
}
