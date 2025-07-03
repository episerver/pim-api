# PIM gRPC API integration guide

## Getting Started

To use the PIM gRPC API provided in this repositor, please follow the guide below:

**Get the required file and libraries**
1. proto file
Download the file from below link, include it to the project by adding an entry to the <Protobuf> item group
https://github.com/episerver/pim-api/blob/main/src/PimApi/Protos/dataIntegration.v1.proto

```xml
<ItemGroup>
  <Protobuf Include="\Protos\dataIntegration.v1.proto" GrpcServices="Client" />
</ItemGroup>
```
2. Add required nuget packages
grpc.net.client (https://www.nuget.org/packages/Grpc.Net.Client) - contains the .netcore client.
google.protobuf (https://www.nuget.org/packages/Google.Protobuf/) - contains protobuf message API for c#: 
grpc.tools (https://www.nuget.org/packages/Grpc.Tools/) - contains c# tooling support for proto files. It translates gRPC calls in proto file into metholds on the concrete type, which can be called directly from your codebase. For example, in dataIntegration.v1.proto, a concreate DataIntegration.DataIntegrationClient type is generated. You can call DataIntegration.DataIntegrationClient.GetImportStatusAsync to initiate a gRPC call to the server.
**Turnstile key and secret**

Sending an email to Optimizely Support team at support@optmizely.com to accquire the AppKey and AppSecret.

**NOTE** 
* Credentials provided will have read-only or write access.
* **read-only** credentials are limited to HTTP GET operations, and **write** credentials will allow you to call data integration endpoints to import data and check import status.

Once credentials have been obtained, create a document in the solution root folder called **ConnectionInformation.json** from the following sample code

```json
{
  "AppKey": "Enter the App Key",
  "AppSecret": "Enter the App Secret"
}
```
**Make a gRPC request to PIM**
1. Build Epi-HMAC authentication token for request’s header
Take reference from the below code sample to build the HMAC authentication 
```c#
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
```
Reference link: https://github.com/episerver/pim-api/blob/7f3c764f85a2715610de3390230c9f27cf7797cb/src/PimApi/GrpcClientService.cs#L72-L98 
Code guide:
-	methodName: the name of the gRPC method you plan to call, example: "GetImportStatus" or "Import."
-	turnstileKey and turnstileSecret: get from above.
2. Call the gRPC API
Take reference from the below code sample to call to PIM gRPC API
```c#
  using var channel = GrpcChannel.ForAddress(PimUrl);
  var client = new DataIntegration.DataIntegrationClient(GrpcChannel.ForAddress(PimUrl));

  Metadata metadata = GenerateMetadata("GetImportStatus", AppKey, AppSecret);
  return await client.GetImportStatusAsync(
      new GetImportStatusRequest { RequestId = requestId },
      metadata
  );
```
Reference: https://github.com/episerver/pim-api/blob/7f3c764f85a2715610de3390230c9f27cf7797cb/src/PimApi/GrpcClientService.cs#L60-L70

## Requirements

Any applications using this PIM API codebase must support at least one of the following

* [.NET Standard 2.0 ](https://docs.microsoft.com/en-us/dotnet/standard/net-standard) (includes cross platform support with .NET Core)
* [.NET Framework 4.6.1](https://www.microsoft.com/en-us/download/details.aspx?id=49981) (Windows only)
* [Postman](https://docs.developers.optimizely.com/digital-experience-platform/docs/authentication#sample-code-to-request-hmac)

## Important Notes

* API uses [OData](https://www.odata.org/)
* Queryies that specify a $top will never include a next link.
* Queries that do not specify $top will return at most 1,000 results. If more are available a next link will be provided for pagination in the response.
* The console application does not demonstrate iterating a full product set, but sample code is available in the EntityIteratorTests.cs class in the test project.
* JSON Serialization can be done with either [Newtonsoft.Json](https://www.nuget.org/packages/Newtonsoft.Json/) or [System.Text.Json](https://www.nuget.org/packages/System.Text.Json/) implemented using a custom IJsonSerializer interface in the Pim API Project.

## Entities

The following PIM entities are available in this API:

* Assets
* CategoryTree
* Product [OData Open Type](https://docs.microsoft.com/en-us/aspnet/web-api/overview/odata-support-in-aspnet-web-api/odata-v4/use-open-types-in-odata-v4)
* Property
* PropertyGroup
* Template
* Website

## Serializers

Both [Newtonsoft.Json](https://www.nuget.org/packages/Newtonsoft.Json/) and [System.Text.Json](https://www.nuget.org/packages/System.Text.Json/) are supported. Basic serialization and deserialization is available in the PimApi.Serialization namespace. Entities do not utilize any special attributes that each serializer provides to allow for choice. If a different serializer is desired an IJsonSerializer interface is provided for customization, but will not be supported.

## Known Issues

* [OData SQL Query Generation](https://github.com/dotnet/efcore/issues/24877)

```cs
 // example of request with nested $expand that should work but does not
 // the workaround is to not use nested $expands and do a second request to get extra data
 var shouldWorkButFails = '/products?$expand=categorytrees($expand=categorytree($select=name,id))';
```

## Useful Resources

* [OData](https://www.odata.org/)
* [OData Overview](https://docs.microsoft.com/en-us/odata/overview)
* [Querying ODATA Endpoints](https://docs.microsoft.com/en-us/odata/webapi/first-odata-api#query-resources-using-odata)
