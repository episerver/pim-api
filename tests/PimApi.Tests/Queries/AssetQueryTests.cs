using FluentAssertions;
using NUnit.Framework;
using PimApi.ConsoleApp.Queries.Asset;
using PimApi.Entities;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using static PimApi.Tests.TestSetup;

namespace PimApi.Tests.Queries
{
    [ExcludeFromCodeCoverage]
    [TestFixture(Category = nameof(AssetDto))]
    [Parallelizable(ParallelScope.All)]
    public class AssetQueryTests
    {
        [TestCase(SystemTextJsonSerializer)]
        [TestCase(NewtonsoftJsonSerializer)]
        public async Task ShouldResolveAndDeserializeExternalImages(string serializerKey)
        {
            var jsonSerializer = serializerKey.GetJsonSerializer();
            var query = new GetAssets
            {
                Top = 10,
                ExternalOnly = true,
                ImageFilter = true
            };

            using var result = query.Execute(ApiClient);

            await query.ShouldRenderMessage(result, jsonSerializer);
            var entities = await result.GetDataAsync<ODataResponseCollection<AssetDto>>(jsonSerializer);
            entities.Should().NotBeNull();

            if (entities.Count == 0) { Assert.Inconclusive(); return; }

            foreach (var entity in entities.Value)
            {
                entity.ExternalFileUrl.Should().NotBeNull();
            }
        }

        [TestCase(SystemTextJsonSerializer)]
        [TestCase(NewtonsoftJsonSerializer)]
        [Test]
        public async Task ShouldResolveAndDeserializeExternalNonImageAssets(string serializerKey)
        {
            var jsonSerializer = serializerKey.GetJsonSerializer();
            var query = new GetAssets
            {
                Top = 10,
                ExternalOnly = true,
                ImageFilter = false
            };

            using var result = query.Execute(ApiClient);

            await query.ShouldRenderMessage(result, jsonSerializer);
            var entities = await result.GetDataAsync<ODataResponseCollection<AssetDto>>(jsonSerializer);
            entities.Should().NotBeNull();

            if (entities.Count == 0) { Assert.Inconclusive(); return; }

            foreach (var entity in entities.Value)
            {
                entity.AssetType.Should().Be("other");
            }
        }

        [TestCase(SystemTextJsonSerializer, "jpg")]
        [TestCase(NewtonsoftJsonSerializer, "jpg")]
        [TestCase(SystemTextJsonSerializer, null)]
        [TestCase(NewtonsoftJsonSerializer, null)]
        public async Task ShouldResolveAndDeserializeJpgImages(
            string serializerKey,
            string fileExtension)
        {
            var jsonSerializer = serializerKey.GetJsonSerializer();
            var query = new GetAssetsByFileExtension
            {
                Top = 10,
                Skip = 0,
                FileExtension = fileExtension,
            };

            using var result = query.Execute(ApiClient);

            await query.ShouldRenderMessage(result, jsonSerializer);
            var entities = await result.GetDataAsync<ODataResponseCollection<AssetDto>>(jsonSerializer);
            entities.Should().NotBeNull();

            if (entities.Count == 0) { Assert.Inconclusive(); return; }

            foreach (var entity in entities.Value)
            {
                entity.FileExtension.Should().Be(query.FileExtension ?? "jpg");
            }
        }

        [TestCase(SystemTextJsonSerializer)]
        [TestCase(NewtonsoftJsonSerializer)]
        public async Task ShouldResolveAndDeserializeQueries(string serializerKey)
        {
            var jsonSerializer = serializerKey.GetJsonSerializer();
            var query = new GetAssets
            {
                Skip = 0,
                Top = 10,
            };

            using var result = query.Execute(ApiClient);

            await query.ShouldRenderMessage(result, jsonSerializer);
            var entities = await result.GetDataAsync<ODataResponseCollection<AssetDto>>(jsonSerializer);
            entities.Should().NotBeNull();

            var queryById = new GetByAssetId
            {
                Id = entities.Value.FirstOrDefault()!.Id
            };

            var entity = await queryById
                .GetEntityById<GetByAssetId, AssetDto>(jsonSerializer);

            entity.Name.Should().NotBeNullOrWhiteSpace();

            // todo: why is external true but the property empty?
            //if (entity.IsExternal)
            //{
            //    entity.ExternalFileUrl.Should().NotBeNullOrWhiteSpace();
            //}
            //else
            //{
            //    entity.InternalFileName.Should().NotBeNullOrWhiteSpace();
            //}

            entity.UrlSmall.Should().NotBeNullOrWhiteSpace();
            entity.UrlMedium.Should().NotBeNullOrWhiteSpace();
            entity.UrlLarge.Should().NotBeNullOrWhiteSpace();
            entity.AssetType.Should().NotBeNullOrWhiteSpace();
        }
    }
}