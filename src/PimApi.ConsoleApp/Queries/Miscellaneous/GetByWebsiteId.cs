using PimApi.ConsoleApp.Renderers.Miscellaneous;

namespace PimApi.ConsoleApp.Queries.Miscellaneous;

[Display(
    GroupName = nameof(Miscellaneous),
    Order = 905,
    Description = "Gets a single website by given Id (Guid)"
)]
public class GetByWebsiteId : IQuery, IQueryWithMessageRenderer, IQueryWithEntityId
{
    public IApiResponseMessageRenderer MessageRenderer => WebsiteDetailRenderer.Default;

    public Guid? Id { get; set; }

    public ApiResponseMessage Execute(HttpClient pimApiClient) =>
        pimApiClient.GetAsync(
            new ODataQuery<WebsiteDto>
            {
                Id = this.GetIdValue(),
                Expand = $"{nameof(WebsiteDto.CategoryTaxonomies)}($count=true)"
            }
        );
}
