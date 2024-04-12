using PimApi.ConsoleApp.Renderers.Property;

namespace PimApi.ConsoleApp.Queries.Property;

[Display(
    GroupName = nameof(Property),
    Order = 311,
    Description = "Searches properties mapped to a commerce field ordered by name"
)]
public class SearchPropertiesMappedToField : IQuery, IQueryWithMessageRenderer
{
    public const string DefaultMappedField = "_Attribute";

    public string? MappedToField { get; set; }

    IApiResponseMessageRenderer IQueryWithMessageRenderer.MessageRenderer =>
        PropertyListRenderer.Default;

    public ApiResponseMessage Execute(HttpClient pimApiClient)
    {
        var fieldMapName =
            this.MappedToField
            ?? Program.ReadValue($"Please enter mapped field name", DefaultMappedField);

        return pimApiClient.GetAsync(
            new ODataQuery<PropertyDto>
            {
                Count = true,
                OrderBy = nameof(PropertyDto.Name),
                Filter = $"propertyMappings/any(p: p/mappedToIscField eq '{fieldMapName}')",
                Expand = nameof(PropertyDto.PropertyMappings)
            }
        );
    }
}
