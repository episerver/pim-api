using PimApi.ConsoleApp.Renderers.Property;

namespace PimApi.ConsoleApp.Queries.Property;

[Display(
    GroupName = nameof(Property),
    Order = 312,
    Description = "Shows properties contained in list of property names, ordered by name"
)]
public class SearchPropertiesByNameInList : IQuery, IQueryWithTopSkip, IQueryWithMessageRenderer
{
    public int? Top { get; set; }

    public string[] PropertyNames { get; set; } = Array.Empty<string>();

    public int? Skip { get; set; }

    public IApiResponseMessageRenderer MessageRenderer => PropertyListRenderer.Default;

    public ApiResponseMessage Execute(HttpClient pimApiClient)
    {
        var values = this.PropertyNames;

        if (values.Length == 0)
        {
            values = Program
                .ReadValue("Please enter property names to find (comma delimited):", string.Empty)
                .Split(',', StringSplitOptions.RemoveEmptyEntries);
        }

        return pimApiClient.GetAsync(
            new ODataQuery<PropertyDto>
            {
                Count = true,
                Top = this.GetTopValue(),
                Skip = this.GetSkipValue(),
                Filter = $"name in ({GetPropertyNames(values)})",
                OrderBy = nameof(PropertyDto.Name)
            }
        );
    }

    private static string GetPropertyNames(string[]? values) =>
        values is null || values.Length == 0
            ? "''"
            : string.Join(',', values.Select(o => $"'{o.Trim('\'')}'"));
}
