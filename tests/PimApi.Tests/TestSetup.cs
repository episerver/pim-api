using NUnit.Framework;
using PimApi.Extensions;
using PimApi.JsonSerialization;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net.Http;
using static PimApi.ConsoleApp.Program;

namespace PimApi.Tests
{
    [ExcludeFromCodeCoverage]
    [SetUpFixture]
    public class TestSetup
    {
        internal const string SystemTextJsonSerializer = nameof(SystemTextJsonSerializer);
        internal const string NewtonsoftJsonSerializer = nameof(NewtonsoftJsonSerializer);
        internal static HttpClient ApiClient { get; private set; } = null!;
        internal static IReadOnlyDictionary<string, IJsonSerializer> JsonSerializers = null!;

        internal static ApiHttpClientFactory TestApiHttpClientFactory = null!;

        [OneTimeSetUp]
        public void Setup()
        {
            IsUnitTest = true;
            JsonSerializers = new Dictionary<string, IJsonSerializer>
            {
                { SystemTextJsonSerializer, new SystemTextJsonSerialzer() },
                { NewtonsoftJsonSerializer, new NewtonsoftJsonSerializer() }
            };

            var connectionFile = new FileInfo(Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                ConnectionInformationFilePath));
            var connectionInformation = connectionFile
                .GetConnectionInformation(JsonSerializers[SystemTextJsonSerializer])
                .Result;

            TestApiHttpClientFactory = new ApiHttpClientFactory(connectionInformation);
            ApiClient = TestApiHttpClientFactory.Create();
        }
    }
}