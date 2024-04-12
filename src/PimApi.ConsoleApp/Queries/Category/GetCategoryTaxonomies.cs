using PimApi.ConsoleApp.Renderers.Category;

namespace PimApi.ConsoleApp.Queries.Category;

/// <summary>Root level for CategoryTaxonomy entities</summary>
[Display(
    GroupName = nameof(Category),
    Order = 150,
    Description = "Gets category taxonomies ordered by name"
)]
public class GetAllCategoryTaxonomies : IQuery, IQueryWithMessageRenderer
{
    public IApiResponseMessageRenderer MessageRenderer => CategoryTaxonomyListRenderer.Default;

    public ApiResponseMessage Execute(HttpClient pimApiClient) =>
        pimApiClient.GetAsync(
            new ODataQuery<CategoryTaxonomyDto>
            {
                Count = true,
                OrderBy = nameof(CategoryTaxonomyDto.Name),
                Expand = nameof(CategoryTaxonomyDto.Websites)
            }
        );
}
