using PimApi.ConsoleApp.Renderers.Miscellaneous;

namespace PimApi.ConsoleApp.Queries.Miscellaneous;

[Display(
    GroupName = nameof(Miscellaneous),
    Order = 900,
    Description = "Shows all websites ordered by name"
)]
public class GetWebsites : IQuery, IQueryWithMessageRenderer
{
    public IApiResponseMessageRenderer MessageRenderer => WebsiteListRenderer.Default;

    public ApiResponseMessage Execute(HttpClient pimApiClient)
    {
        return pimApiClient.GetAsync(
            new ODataQuery<WebsiteDto>
            {
                Count = true,
                OrderBy = $"{nameof(WebsiteDto.Name)}",
                Expand = $"{nameof(WebsiteDto.CategoryTaxonomies)}"
            }
        );
    }
}
