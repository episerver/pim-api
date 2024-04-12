using PimApi.ConsoleApp.Renderers.Category;

namespace PimApi.ConsoleApp.Queries.Category;

[Display(
    GroupName = nameof(Category),
    Order = 100,
    Description = "Gets category trees ordered by display sequence"
)]
public class GetCategoryTrees
    : IQuery,
        IQueryWithMessageRenderer,
        IQueryWithTopSkip,
        IQueryWithParentId
{
    public int? Top { get; set; }

    public int? Skip { get; set; }

    public Guid? ParentId { get; set; }

    public Guid? CategoryTaxonomyTreeId { get; set; }

    IApiResponseMessageRenderer IQueryWithMessageRenderer.MessageRenderer =>
        CategoryTreeListRenderer.Default;

    public ApiResponseMessage Execute(HttpClient pimApiClient)
    {
        var filter = this.GetParentIdValue() is not Guid parentId
            ? $"{nameof(CategoryTreeDto.ParentId)} eq null"
            : $"{nameof(CategoryTreeDto.ParentId)} eq {parentId}";

        var taxonomyTreeId =
            this.CategoryTaxonomyTreeId
            ?? Program.ReadValue<Guid?>("Please enter category taxonomy Id:", null);

        filter += taxonomyTreeId is null
            ? string.Empty
            : $" and {nameof(CategoryTreeDto.CategoryTaxonomyId)} eq {taxonomyTreeId}";

        return pimApiClient.GetAsync(
            new ODataQuery<CategoryTreeDto>
            {
                Filter = filter,
                Count = true,
                Expand =
                    $"{nameof(CategoryTreeDto.CategoryTaxonomy)}($select=name),"
                    + $"{nameof(CategoryTreeDto.Products)}($count=true;$top=1),"
                    + $"{nameof(CategoryTreeDto.Properties)}($count=true;$top=1),"
                    + $"{nameof(CategoryTreeDto.ChildCategoryTrees)}($count=true;$select=name)",
                Top = this.GetTopValue(),
                Skip = this.GetSkipValue(),
                OrderBy = nameof(CategoryTreeDto.DisplaySequence)
            }
        );
    }
}
