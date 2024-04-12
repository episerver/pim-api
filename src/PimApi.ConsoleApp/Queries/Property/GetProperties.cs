using PimApi.ConsoleApp.Renderers.Property;

namespace PimApi.ConsoleApp.Queries.Property;

[Display(GroupName = nameof(Property), Order = 300, Description = "Shows properties")]
public class GetProperties : IQuery, IQueryWithMessageRenderer, IQueryWithTopSkip
{
    public int? Top { get; set; }

    public int? Skip { get; set; }

    IApiResponseMessageRenderer IQueryWithMessageRenderer.MessageRenderer =>
        PropertyListRenderer.Default;

    public ApiResponseMessage Execute(HttpClient pimApiClient) =>
        pimApiClient.GetAsync(
            new ODataQuery<PropertyDto>
            {
                Count = true,
                Top = this.GetTopValue(),
                Skip = this.GetSkipValue(),
                OrderBy = nameof(PropertyDto.DisplaySequence)
            }
        );
}
