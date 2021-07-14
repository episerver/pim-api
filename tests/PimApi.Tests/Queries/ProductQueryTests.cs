using FluentAssertions;
using NUnit.Framework;
using PimApi.ConsoleApp.Queries.Product;
using PimApi.Entities;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using static PimApi.Tests.TestSetup;

namespace PimApi.Tests.Queries
{
    [ExcludeFromCodeCoverage]
    [TestFixture(Category = nameof(ProductDto))]
    [Parallelizable(ParallelScope.All)]
    public class ProductQueryTests
    {
        private const string nullString = "null";

        [TestCase(SystemTextJsonSerializer, nameof(ProductDto.ProductAssets))]
        [TestCase(NewtonsoftJsonSerializer, nameof(ProductDto.ProductAssets))]
        [TestCase(SystemTextJsonSerializer, nameof(ProductDto.CategoryTrees))]
        [TestCase(NewtonsoftJsonSerializer, nameof(ProductDto.CategoryTrees))]
        public async Task GetProductsByProductNumberShouldResolveAndDeserialize(
            string serializerKey,
            string expandName)
        {
            var jsonSerializer = serializerKey.GetJsonSerializer();
            var query = new GetProductsByProductNumber
            {
                // when Top is used nextlink is disabled
                Top = 1,
                Skip = 0,
                // IMPORTANT: Due to an OData Bug nested expands currently do not work in .NET 5.0, example below
                // expand: $"{nameof(ProductDto.CategoryTrees)}($expand=categorytree($select=name,id))"
                // see this for details: https://github.com/dotnet/efcore/issues/24877
                Expand = expandName,
                Filter = $"{expandName}/any()"
            };

            using var response = query.Execute(ApiClient);

            await query.ShouldRenderMessage(response, jsonSerializer);
            var responseData = await response.GetResponseData<ProductDto>(jsonSerializer);
            responseData.Should().NotBeNull();
            responseData.Value.Count.Should().Be(1);
            var product = responseData.Value.First();
            product.ProductNumber.Should().NotBeNull();

            switch (expandName)
            {
                case nameof(ProductDto.ProductAssets):
                    product.ProductAssets.Should().NotBeEmpty();
                    break;
                case nameof(ProductDto.CategoryTrees):
                    product.CategoryTrees.Should().NotBeEmpty();
                    break;
                default: throw new Exception();
            }
        }

        [TestCase(SystemTextJsonSerializer)]
        [TestCase(NewtonsoftJsonSerializer)]
        public async Task GetByProductIdShouldResolveAndDeserializeData(string serializerKey)
        {
            var jsonSerializer = serializerKey.GetJsonSerializer();
            var productToSearch = await jsonSerializer.GetFirstProduct();
            var queryById = new GetByProductId
            {
                Id = productToSearch.Id
            };

            var entity = await queryById.GetEntityById<GetByProductId,ProductDto>(jsonSerializer);

            entity.ProductNumber.Should().Be(productToSearch.ProductNumber);
            entity.PropertyBag.Count.Should().BeGreaterThan(0);
        }

        [TestCase(SystemTextJsonSerializer, nameof(ProductDto.CreatedOn))]
        [TestCase(NewtonsoftJsonSerializer, nameof(ProductDto.CreatedOn))]
        [TestCase(SystemTextJsonSerializer, nameof(ProductDto.ModifiedOn))]
        [TestCase(NewtonsoftJsonSerializer, nameof(ProductDto.ModifiedOn))]
        public async Task SearchByDateFieldByNDaysShouldResolveAndDeserializeData(string serializerKey, string dateField)
        {
            var jsonSerializer = serializerKey.GetJsonSerializer();
            var query = new SearchByDateFieldLastNDays
            {
                Top = 1,
                Skip = 0,
                DateField = dateField,
                PreviousDays = 500
            };

            using var response = query.Execute(ApiClient);

            await query.ShouldRenderMessage(response, jsonSerializer);
            var responseData = await response.GetResponseData<ProductDto>(jsonSerializer);
            responseData.Should().NotBeNull();
            responseData.Value.Count.Should().Be(1);
            responseData.Value.First().ProductNumber
                .Should()
                .NotBeNullOrWhiteSpace();
        }

        [TestCase(SystemTextJsonSerializer)]
        [TestCase(NewtonsoftJsonSerializer)]
        public async Task SearchByProductNumberShouldResolveAndDeserializeData(string serializerKey)
        {
            var jsonSerializer = serializerKey.GetJsonSerializer();
            var productToSearch = await jsonSerializer.GetFirstProduct();
            var query = new SearchByProductPropertyEqualsValue
            {
                Top = 1,
                Skip = 0,
                PropertyToSearch = nameof(ProductDto.ProductNumber),
                ValueToSearch = productToSearch.ProductNumber
            };

            using var response = query.Execute(ApiClient);

            await query.ShouldRenderMessage(response, jsonSerializer);
            var responseData = await response.GetResponseData<ProductDto>(jsonSerializer);
            responseData.Should().NotBeNull();
            responseData.Value.Count.Should().Be(1);
            responseData.Value.First().ProductNumber
                .Should()
                .Be(query.ValueToSearch);
        }

        [TestCase(SystemTextJsonSerializer)]
        [TestCase(NewtonsoftJsonSerializer)]
        public async Task SearchByProductPropertyShouldResolveEqualNullAndDeserializeData(string serializerKey)
        {
            var jsonSerializer = serializerKey.GetJsonSerializer();
            var propertyToSearch = await jsonSerializer.GetFirstProperty();
            var query = new SearchByProductPropertyEqualsValue
            {
                Top = 1,
                Skip = 0,
                PropertyToSearch = propertyToSearch.Name,
                ValueToSearch = nullString
            };

            using var response = query.Execute(ApiClient);

            await query.ShouldRenderMessage(response, jsonSerializer);
            var responseData = await response.GetResponseData<ProductDto>(jsonSerializer);
            responseData.Should().NotBeNull();

            if (responseData.Value.Count == 0)
            {
                // NOTE: Test can fail if first property return does not allow null values
                Assert.Inconclusive();
                return;
            }

            responseData.Value.Count.Should().Be(1);
            var product = responseData.Value.First();

            product.PropertyBag.Should().ContainKey(propertyToSearch.Name);
            product.PropertyBag[propertyToSearch.Name].Should().BeNull();
        }

        [TestCase(SystemTextJsonSerializer)]
        [TestCase(NewtonsoftJsonSerializer)]
        public async Task SearchByProductPropertyShouldResolveNotEqualNullAndDeserializeData(string serializerKey)
        {
            var jsonSerializer = serializerKey.GetJsonSerializer();
            var propertyToSearch = await jsonSerializer.GetFirstProperty();
            var query = new SearchByProductPropertyEqualsValue
            {
                Top = 1,
                Skip = 0,
                PropertyToSearch = propertyToSearch.Name,
                IsNotEqualsFilter = true,
                ValueToSearch = nullString
            };

            using var response = query.Execute(ApiClient);

            await query.ShouldRenderMessage(response, jsonSerializer);
            var responseData = await response.GetResponseData<ProductDto>(jsonSerializer);
            responseData.Should().NotBeNull();
            responseData.Value.Count.Should().Be(1);
            var product = responseData.Value.First();

            product.PropertyBag.Should().ContainKey(propertyToSearch.Name);
            product.PropertyBag[propertyToSearch.Name].Should().NotBeNull();
        }

        [TestCase(SystemTextJsonSerializer)]
        [TestCase(NewtonsoftJsonSerializer)]
        public async Task SearchByProductCategoryShouldResolveNotEqualNullAndDeserializeData(string serializerKey)
        {
            var jsonSerializer = serializerKey.GetJsonSerializer();
            var categoryWithProducts = await jsonSerializer.GetFirstCategoryTreeWithProducts();
            var query = new SearchProductsByCategory
            {
                Top = 1,
                Skip = 0,
                CategoryId = categoryWithProducts.Id
            };

            using var response = query.Execute(ApiClient);

            await query.ShouldRenderMessage(response, jsonSerializer);
            var responseData = await response.GetResponseData<ProductDto>(jsonSerializer);
            responseData.Should().NotBeNull();
            responseData.Value.Count.Should().Be(1);
            var product = responseData.Value.First();
            product.CategoryTrees
                .Any(o => o.CategoryTreeId == categoryWithProducts.Id)
                .Should()
                .BeTrue();
        }

        [TestCase(SystemTextJsonSerializer)]
        [TestCase(NewtonsoftJsonSerializer)]
        public async Task SearchByActiveProductsShouldResolveAndDeserializeData(string serializerKey)
        {
            var jsonSerializer = serializerKey.GetJsonSerializer();
            var query = new GetActiveProducts
            {
                Top = 1,
                Skip = 1,
            };
            using var response = query.Execute(ApiClient);

            await query.ShouldRenderMessage(response, jsonSerializer);
            var responseData = await response.GetResponseData<ProductDto>(jsonSerializer);
            responseData.Should().NotBeNull();
            responseData!.Value.Count.Should().BeLessOrEqualTo(query.Top.Value);

            foreach (var product in responseData.Value)
            {
                product.Should().NotBeNull();
                product.ProductNumber.Should().NotBeNull();
                product.ProductTitle.Should().NotBeNull();
                product.Status.Should().NotBeNull();
            }
        }
    }
}