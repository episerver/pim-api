using PimApi.ConsoleApp.Queries;
using static PimApi.Tests.TestSetup;

namespace PimApi.Tests
{
    [ExcludeFromCodeCoverage]
    internal static class TestExtensions
    {
        private static ProductDto? firstProduct;
        private static CategoryTreeDto? firstCategory;
        private static PropertyDto? firstProperty;

        public static IJsonSerializer GetJsonSerializer(this string s) =>
            JsonSerializers.TryGetValue(s, out var jsonSerializer)
                ? jsonSerializer
                : throw new Exception("Unable to find serializer for " + s);

        public static async Task<ODataResponseCollection<TEntity>> ThrowIfNull<TEntity>(
            this Task<ODataResponseCollection<TEntity>> oDataResponseEntityCollection)
            where TEntity : BaseEntityDto, new()
        {
            var result = await oDataResponseEntityCollection;

            return result is null
                ? throw new ArgumentNullException(nameof(TEntity))
                : result;
        }

        internal static async Task ShouldRenderMessage(
            this IQueryWithMessageRenderer query,
            ApiResponseMessage apiResponseMessage,
            IJsonSerializer jsonSerializer)
        {
            var success = await apiResponseMessage.IsSuccessful();
            success.Should().BeTrue();

            var stringBuilder = new StringBuilder();
            await query.MessageRenderer.Render(
                apiResponseMessage,
                jsonSerializer,
                s => stringBuilder.AppendLine(s));
            stringBuilder.Length.Should().BeGreaterThan(0);
        }

        internal static async Task<CategoryTreeDto> GetFirstCategoryTreeWithProducts(this IJsonSerializer jsonSerializer)
        {
            if (firstCategory is not null) { return firstCategory; }

            var query = new ODataQuery<CategoryTreeDto>
            {
                Top = 1,
                Select = $"{nameof(CategoryTreeDto.Name)},{nameof(CategoryTreeDto.Id)}",
                Filter = $"{nameof(CategoryTreeDto.Products)}/any()",
                OrderBy = nameof(CategoryTreeDto.Id)
            };
            var responseMessage = new ApiResponseMessage(ApiClient.GetAsync(query));
            var responseData = await responseMessage.GetResponseData<CategoryTreeDto>(jsonSerializer);
            responseData.Should().NotBeNull();
            responseData.Value.Count.Should().BeGreaterThan(0);

            var category = responseData.Value.FirstOrDefault()
                ?? throw new Exception("Unable to find an category with products for test purposes");

            category.Name.Should().NotBeEmpty();

            return firstCategory = category;
        }

        internal static async Task<ProductDto> GetFirstProduct(this IJsonSerializer jsonSerializer)
        {
            if (firstProduct is not null) { return firstProduct; }

            var query = new ODataQuery<ProductDto>
            {
                Top = 1,
                Select = $"{nameof(ProductDto.ProductNumber)},{nameof(ProductDto.Id)}",
                OrderBy = nameof(ProductDto.ProductNumber)
            };
            var responseMessage = new ApiResponseMessage(ApiClient.GetAsync(query));
            var responseData = await responseMessage.GetResponseData<ProductDto>(jsonSerializer);
            responseData.Should().NotBeNull();
            responseData.Value.Count.Should().BeGreaterThan(0);

            var product = responseData.Value.FirstOrDefault()
                ?? throw new Exception("Unable to find an active product for test purposes");

            // demonstrates Select only got productNumber
            product.Status.Should().BeEmpty();
            product.ProductNumber.Should().NotBeNullOrWhiteSpace();

            return firstProduct = product;
        }

        internal static async Task<PropertyDto> GetFirstProperty(this IJsonSerializer jsonSerializer)
        {
            var query = new ODataQuery<PropertyDto>
            {
                Top = 1,
                Select = $"{nameof(PropertyDto.Name)},{nameof(PropertyDto.Id)}",
                OrderBy = nameof(PropertyDto.Name)
            };
            var responseMessage = new ApiResponseMessage(ApiClient.GetAsync(query));
            var propertiesToSearch = await responseMessage.GetResponseData<PropertyDto>(jsonSerializer);
            propertiesToSearch.Should().NotBeNull();
            propertiesToSearch.Value.Count.Should().BeGreaterThan(0);

            var entity = propertiesToSearch.Value.FirstOrDefault()
                ?? throw new Exception("Unable to find an active product for test purposes");

            // demonstrates Select only got productNumber
            entity.BooleanLabel.Should().BeEmpty();
            entity.Name.Should().NotBeNullOrWhiteSpace();

            return firstProperty = entity;
        }

        internal static async Task<ODataResponseCollection<TEntity>> GetResponseData<TEntity>(
            this ApiResponseMessage response,
            IJsonSerializer jsonSerializer)
            where TEntity : BaseEntityDto, new()
            => await response
                .GetDataAsync<ODataResponseCollection<TEntity>>(jsonSerializer)
                .ThrowIfNull();

        internal static async Task<TEntity> GetEntityById<TRenderer,TEntity>(
            this TRenderer query,
            IJsonSerializer jsonSerializer)
            where TRenderer : IQueryWithEntityId, IQueryWithMessageRenderer
            where TEntity : BaseEntityDto, new()
        {
            if (!query.Id.HasValue) { throw new Exception(); }
            using var result = query.Execute(ApiClient);

            await query.ShouldRenderMessage(result, jsonSerializer);
            var entity = await result.GetDataAsync<TEntity>(jsonSerializer);
            entity.Should().NotBeNull();
            entity.Id.Should().Be(query.Id.Value);

            return entity;
        }
    }
}