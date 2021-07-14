using FluentAssertions;
using NUnit.Framework;
using PimApi.ConsoleApp;
using PimApi.ConsoleApp.Queries;
using PimApi.ConsoleApp.Renderers.Product;
using PimApi.Entities;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static PimApi.Tests.TestSetup;

namespace PimApi.Tests
{
    [ExcludeFromCodeCoverage]
    [TestFixture(Category = nameof(EntityIteratorTests))]
    [Parallelizable(ParallelScope.All)]
    public class EntityIteratorTests
    {
        [TestCase(SystemTextJsonSerializer)]
        [TestCase(NewtonsoftJsonSerializer)]
        [Test]
        public async Task EntityIteratorShouldDeserializeODataResponseForAllProducts(string serializerKey)
        {
            var tokenSource = new CancellationTokenSource();
            var jsonSerializer = serializerKey.GetJsonSerializer();
            var iterations = 0;
            var productIterator = new EntityIterator<ProductDto>(
                ApiClient,
                jsonSerializer,
                new ProductListRenderer())
            {
                TestAssertions =
                async (query, response) =>
                     await AssertProductData(query, response, jsonSerializer)
            };

            var query = new ODataQuery<ProductDto>
            {
                Select = nameof(ProductDto.ProductNumber),
                Count = true
            };

            await foreach (var product in productIterator.GetEntities(query, tokenSource.Token))
            {
                iterations++;
                product.ProductNumber.Should().NotBeNullOrWhiteSpace();
            }

            iterations.Should().Be(productIterator.TotalEntitiesForQuery);
            productIterator.NumberOfRequests.Should().BeGreaterThan(0);
        }

        private static async Task AssertProductData(
            IQuery query,
            ApiResponseMessage apiResponseMessage,
            IJsonSerializer jsonSerializer)
        {
            if (query is not IQueryWithMessageRenderer queryRenderer)
            {
                throw new ArgumentException($"Given query does not implement {nameof(IQueryWithMessageRenderer)}!");
            }

            await queryRenderer.ShouldRenderMessage(apiResponseMessage, jsonSerializer);
            var productList = await apiResponseMessage.GetDataAsync<ODataResponseCollection<ProductDto>>(jsonSerializer);
            productList.Should().NotBeNull();
            productList.Context.Should().NotBeNull();
            productList.Count.Should().NotBeNull();
            productList.Count.Should().BeGreaterThan(0);

            var httpResponseMessage = await apiResponseMessage.GetHttpResponseMessage();
            var queryParts = httpResponseMessage.RequestMessage?.RequestUri?.Query
                .Split('&', StringSplitOptions.RemoveEmptyEntries)
                ?? Array.Empty<string>();

            // only test next url if skip is present and skip + 1k is less than count
            var skip = queryParts.FirstOrDefault(o => o.StartsWith("$skip="));
            if (skip is null) { return; }
            var equalsignIndex = skip.IndexOf('=');
            var skipped = int.Parse(skip[(equalsignIndex + 1)..]);
            if (skipped + Defaults.MaxPageSize > productList.Count) { return; }

            productList.NextLink.Should().NotBeNull();
        }
    }
}