using FluentAssertions;
using NUnit.Framework;
using PimApi.Extensions;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using static PimApi.ConsoleApp.Program;
using static PimApi.Tests.TestSetup;

namespace PimApi.Tests
{
    [ExcludeFromCodeCoverage]
    [TestFixture(Category = nameof(ConnectionInformation))]
    [Parallelizable(ParallelScope.All)]
    public class ConnectionInformationTests
    {
        [TestCase(SystemTextJsonSerializer)]
        [TestCase(NewtonsoftJsonSerializer)]
        [Test]
        public async Task ShouldReadSampleJson(string serializerKey)
        {
            var jsonSerializer = JsonSerializers[serializerKey];
            var filePath = ConnectionInformationFilePath.Replace(".json", ".Sample.json");
            var fileInfo = new FileInfo(filePath);

            var connectionInformation = await fileInfo.GetConnectionInformation(jsonSerializer);

            connectionInformation.Should().NotBeNull();
            connectionInformation.AppKey.Should().Be("Enter AppKey");
            connectionInformation.AppSecret.Should().Be("Enter AppSecret");
        }

        [TestCase(null)]
        [TestCase("http://somethignbad")]
        [Test]
        public void ShouldEnsureHttpsAuthUrl(string? badUrl)
        {
            var connectionInformation = new ConnectionInformation
            {
                AuthUrl = badUrl!,
                PimUrlBase = string.Empty
            };

            connectionInformation.EnsureConnectionInformation();

            connectionInformation.AuthUrl.Should().NotBe(badUrl);
            connectionInformation.PimUrlBase.Should().NotBeNullOrWhiteSpace();
            connectionInformation.PimUrlBase.Length.Should().BeGreaterThan(0);
        }

        [Test]
        public async Task ShouldThrowExceptionWhenFileInfoDoesNotExist()
        {
            var fileInfo = new FileInfo(nameof(ShouldThrowExceptionWhenFileInfoDoesNotExist));

            Func<Task> act = () => fileInfo.GetConnectionInformation(null!);

            await act.Should().ThrowAsync<Exception>();
        }
    }
}