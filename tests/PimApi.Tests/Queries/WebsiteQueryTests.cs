using PimApi.ConsoleApp.Queries.Miscellaneous;
using static PimApi.Tests.TestSetup;

namespace PimApi.Tests.Queries;

[ExcludeFromCodeCoverage]
[TestFixture(Category = nameof(WebsiteDto))]
[Parallelizable(ParallelScope.All)]
public class WebsiteQueryTests
{
    [TestCase(SystemTextJsonSerializer)]
    [TestCase(NewtonsoftJsonSerializer)]
    public async Task ShouldResolveAndDeserializeQueries(string serializerKey)
    {
        var jsonSerializer = serializerKey.GetJsonSerializer();
        var query = new GetWebsites();

        using var result = query.Execute(ApiClient);

        await query.ShouldRenderMessage(result, jsonSerializer);
        var websites = await result.GetDataAsync<ODataResponseCollection<WebsiteDto>>(
            jsonSerializer
        );
        websites.Should().NotBeNull();

        if (websites.Value.Count == 0)
        {
            Assert.Inconclusive();
            return;
        }

        var queryById = new GetByWebsiteId { Id = websites.Value.FirstOrDefault()!.Id };
        var entity = await queryById.GetEntityById<GetByWebsiteId, WebsiteDto>(jsonSerializer);

        entity.IscWebsiteId.Should().NotBeEmpty();
        entity.Name.Should().NotBeNullOrWhiteSpace();
    }
}
