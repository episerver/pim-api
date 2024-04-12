using PimApi.ConsoleApp.Queries.Property;
using static PimApi.Tests.TestSetup;

namespace PimApi.Tests.Queries;

[ExcludeFromCodeCoverage]
[TestFixture(Category = nameof(PropertyDto))]
[Parallelizable(ParallelScope.All)]
public class PropertyQueryTests
{
    [TestCase(SystemTextJsonSerializer)]
    [TestCase(NewtonsoftJsonSerializer)]
    public async Task GetPropertyGroupsShouldResolveAndDeserialize(string serializerKey)
    {
        var jsonSerializer = serializerKey.GetJsonSerializer();
        var query = new GetPropertyGroups();

        using var result = query.Execute(ApiClient);

        await query.ShouldRenderMessage(result, jsonSerializer);
        var entities = await result.GetDataAsync<ODataResponseCollection<PropertyGroupDto>>(
            jsonSerializer
        );

        entities.Should().NotBeNull();
        entities.Count.Should().BeGreaterThan(0);
        entities.Count.Should().Be(entities.Value.Count);
        entities.Value.First().HasPropertyCount().Should().BeTrue();
        // get it twice to confirm it used ref correctly
        entities.Value.First().PropertyCount().Should().BeGreaterOrEqualTo(0);
        entities.Value.First().PropertyCount().Should().BeGreaterOrEqualTo(0);
    }

    [TestCase(SystemTextJsonSerializer)]
    [TestCase(NewtonsoftJsonSerializer)]
    public async Task ShouldResolveAndDeserializeQueries(string serializerKey)
    {
        var jsonSerializer = serializerKey.GetJsonSerializer();
        var query = new GetProperties { Skip = 0, Top = 10, };

        using var result = query.Execute(ApiClient);

        await query.ShouldRenderMessage(result, jsonSerializer);
        var entities = await result.GetDataAsync<ODataResponseCollection<PropertyDto>>(
            jsonSerializer
        );
        entities.Should().NotBeNull();

        var queryById = new GetByPropertyId { Id = entities.Value.FirstOrDefault()!.Id };

        var entity = await queryById.GetEntityById<GetByPropertyId, PropertyDto>(jsonSerializer);

        entity.Name.Should().NotBeNullOrWhiteSpace();
        entity.DisplayName.Should().NotBeNullOrWhiteSpace();
        entity.Type.Should().NotBeNullOrWhiteSpace();
        entity.ControlType.Should().NotBeNullOrWhiteSpace();
    }

    [TestCase(SystemTextJsonSerializer)]
    [TestCase(NewtonsoftJsonSerializer)]
    public async Task SearchPropertiesMappedToFieldShouldResolveAndDeserialize(string serializerKey)
    {
        var jsonSerializer = serializerKey.GetJsonSerializer();
        var query = new SearchPropertiesMappedToField { MappedToField = "_Attribute" };

        using var result = query.Execute(ApiClient);

        await query.ShouldRenderMessage(result, jsonSerializer);
        var entities = await result.GetDataAsync<ODataResponseCollection<PropertyDto>>(
            jsonSerializer
        );

        entities.Should().NotBeNull();
        entities.Count.Should().Be(entities.Value.Count);
    }

    [TestCase(SystemTextJsonSerializer)]
    [TestCase(NewtonsoftJsonSerializer)]
    public async Task SearchPropertiesByNameInListShouldResolveAndDeserialize(string serializerKey)
    {
        var jsonSerializer = serializerKey.GetJsonSerializer();
        var query = new SearchPropertiesByNameInList { PropertyNames = new[] { "productNumber" } };

        using var result = query.Execute(ApiClient);

        await query.ShouldRenderMessage(result, jsonSerializer);
        var entities = await result.GetDataAsync<ODataResponseCollection<PropertyDto>>(
            jsonSerializer
        );

        entities.Should().NotBeNull();
        entities.Count.Should().BeGreaterThan(0);
        entities.Count.Should().Be(entities.Value.Count);
    }

    [TestCase(SystemTextJsonSerializer)]
    [TestCase(NewtonsoftJsonSerializer)]
    public async Task SearchPropertiesByControlTypeShouldResolveAndDeserialize(string serializerKey)
    {
        var jsonSerializer = serializerKey.GetJsonSerializer();
        var query = new SearchPropertiesByControlType
        {
            Skip = 0,
            Top = 1,
            ControlType = "textfield"
        };

        using var result = query.Execute(ApiClient);

        await query.ShouldRenderMessage(result, jsonSerializer);
        var entities = await result.GetDataAsync<ODataResponseCollection<PropertyDto>>(
            jsonSerializer
        );

        entities.Should().NotBeNull();
        entities.Count.Should().BeGreaterThan(0);
        query.Top.Should().Be(entities.Value.Count);
    }
}
