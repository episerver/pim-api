using PimApi.ConsoleApp.Queries.Miscellaneous;
using static PimApi.Tests.TestSetup;

namespace PimApi.Tests.Queries;

[ExcludeFromCodeCoverage]
[TestFixture(Category = nameof(TemplateDto))]
[Parallelizable(ParallelScope.All)]
public class TemplateQueryTests
{
    [TestCase(SystemTextJsonSerializer)]
    [TestCase(NewtonsoftJsonSerializer)]
    public async Task ShouldResolveAndDeserializeQueries(string serializerKey)
    {
        var jsonSerializer = serializerKey.GetJsonSerializer();
        var query = new GetTemplates { Skip = 0, Top = 10, };

        using var result = query.Execute(ApiClient);

        await query.ShouldRenderMessage(result, jsonSerializer);
        var templates = await result.GetDataAsync<ODataResponseCollection<TemplateDto>>(
            jsonSerializer
        );
        templates.Should().NotBeNull();

        var queryById = new GetByTemplateId { Id = templates.Value.FirstOrDefault()!.Id };
        var entity = await queryById.GetEntityById<GetByTemplateId, TemplateDto>(jsonSerializer);

        entity.Name.Should().NotBeNullOrWhiteSpace();

        if (!entity.IsTemplate)
        {
            return;
        }
        entity.TemplatePropertyGroups.Should().NotBeEmpty();
    }
}
