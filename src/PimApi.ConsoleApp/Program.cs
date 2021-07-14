using PimApi.Extensions;
using PimApi.JsonSerialization;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace PimApi.ConsoleApp
{
    // Partial class is used to only highlight important code in this main file
    [ExcludeFromCodeCoverage]
    public partial class Program
    {
        /// <summary>
        /// Stored in root folder with solution
        /// </summary>
        internal const string ConnectionInformationFilePath = "../../../../../ConnectionInformation.json";

        private static async Task Main(string[] args)
        {
            var jsonSerializer = UseNewtonsoftSerializer(args)
               ? new NewtonsoftJsonSerializer()
               : new SystemTextJsonSerialzer() as IJsonSerializer;

            // see README.md about obtaining credentials from support
            // create a file from ConnectionInformation.Sample.json if one does not exist and set provided AppKey and AppSecret
            // IMPORTANT: Not any HttpClient will work, one needs to be configured with the tokens provided which will set the appropriate HTTP Headers for API requests.
            // If using just a normal HttpClient all requests will be unauthorized!
            var apiClient = await CreateAuthorizedPimApiClientFromFile(GetConnectionFile(args), jsonSerializer);

            await Execute(apiClient, jsonSerializer, GetQueries());
        }

        /// <summary>
        /// Creates http client that sends proper headers for PIM API access
        /// </summary>
        /// <returns></returns>
        private static async Task<HttpClient> CreateAuthorizedPimApiClientFromFile(string fullFilePath, IJsonSerializer jsonSerializer)
        {
            var connectionFile = new FileInfo(fullFilePath);

            // uses an extension method to convert FileInfo into ConnectionInformation class
            var connectionInformation = await connectionFile.GetConnectionInformation(jsonSerializer);

            return new ApiHttpClientFactory(connectionInformation).Create();
        }

        // first argument is path to JSON file
        private static string GetConnectionFile(string[] args) => args.Length > 0
            ? args[0]
            : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConnectionInformationFilePath);

        // 2nd argument is serializer switch
        private static bool UseNewtonsoftSerializer(string[] args) =>
            args.Length > 1 && bool.TryParse(args[1], out var useNewtonsoftSerializer) && useNewtonsoftSerializer;
    }
}