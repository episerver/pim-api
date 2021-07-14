using FluentAssertions;
using NUnit.Framework;
using PimApi.ConsoleApp.Queries.Category;
using PimApi.Entities;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using static PimApi.Tests.TestSetup;

namespace PimApi.Tests.Queries
{
    [ExcludeFromCodeCoverage]
    [TestFixture(Category = nameof(CategoryTaxonomyDto))]
    [Parallelizable(ParallelScope.All)]
    public class CategoryTaxonomyQueryTests
    {
        [TestCase(SystemTextJsonSerializer)]
        [TestCase(NewtonsoftJsonSerializer)]
        public async Task ShouldResolveAndDeserializeQueries(string serializerKey)
        {
            var jsonSerializer = serializerKey.GetJsonSerializer();
            var query = new GetAllCategoryTaxonomies();

            using var result = query.Execute(ApiClient);

            await query.ShouldRenderMessage(result, jsonSerializer);
            var entities = await result.GetDataAsync<ODataResponseCollection<CategoryTaxonomyDto>>(jsonSerializer);
            entities.Should().NotBeNull();

            var queryById = new GetByCategoryTaxonomyId
            {
                Id = entities.Value.FirstOrDefault()!.Id
            };

            var entity = await queryById
                .GetEntityById<GetByCategoryTaxonomyId, CategoryTaxonomyDto>(jsonSerializer);

            entity.Name.Should().NotBeNullOrWhiteSpace();
        }
    }
}