# PIM Samples

## Getting Started

To use the PIM API samples in this project, credentials (AppKey and AppSecret) must first be obtained by contacting Optimizely support at support@optmizely.com.

**IMPORTANT** 
* Credentials provided will have read-only or write access.
* **read-only** credentials are limited to HTTP GET operations, and **write** credentials will allow you to call data integration endpoints to import data and check import status.

Once credentials have been obtained, create a document in the solution root folder called **ConnectionInformation.json** from the following sample code

```json
{
  "AppKey": "Enter the App Key",
  "AppSecret": "Enter the App Secret"
}
```

Then simply start the console application to run the example test queries by entering their query number or name using Visual Studio or the .NET cli command in the checkout out code folder

```txt
dotnet run
```

## Requirements

Any applications using this PIM API codebase must support at least one of the following

* [.NET Standard 2.0 ](https://docs.microsoft.com/en-us/dotnet/standard/net-standard) (includes cross platform support with .NET Core)
* [.NET Framework 4.6.1](https://www.microsoft.com/en-us/download/details.aspx?id=49981) (Windows only)

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