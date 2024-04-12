using PimApi.ConsoleApp.Queries.Category;
using static PimApi.Tests.TestSetup;

namespace PimApi.Tests.Queries;

[ExcludeFromCodeCoverage]
[TestFixture(Category = nameof(CategoryTreeDto))]
[Parallelizable(ParallelScope.All)]
public class CategoryTreeQueryTests
{
    [TestCase(SystemTextJsonSerializer)]
    [TestCase(NewtonsoftJsonSerializer)]
    public async Task ShouldResolveAndDeserializeQueries(string serializerKey)
    {
        var jsonSerializer = serializerKey.GetJsonSerializer();
        var query = new GetCategoryTrees { Skip = 0, Top = 10, };

        using var result = query.Execute(ApiClient);

        await query.ShouldRenderMessage(result, jsonSerializer);
        var entities = await result.GetDataAsync<ODataResponseCollection<CategoryTreeDto>>(
            jsonSerializer
        );
        entities.Should().NotBeNull();

        var queryById = new GetByCategoryTreeId { Id = entities.Value.FirstOrDefault()!.Id };

        var entity = await queryById.GetEntityById<GetByCategoryTreeId, CategoryTreeDto>(
            jsonSerializer
        );

        entity.Name.Should().NotBeNullOrWhiteSpace();
        entity.UrlSegment.Should().NotBeNullOrWhiteSpace();
        entity.DisplaySequence.Should().BeGreaterOrEqualTo(0);
    }
}
