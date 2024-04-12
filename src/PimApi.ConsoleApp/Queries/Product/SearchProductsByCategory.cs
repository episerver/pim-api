using PimApi.ConsoleApp.Renderers.Product;

namespace PimApi.ConsoleApp.Queries.Product;

[Display(
    GroupName = nameof(Product),
    Order = 10,
    Description = "Searches products in a category Id, ordered by productNumber"
)]
public partial class SearchProductsByCategory : IQuery, IQueryWithMessageRenderer, IQueryWithTopSkip
{
    public int? Top { get; set; }

    public int? Skip { get; set; }

    public Guid? CategoryId { get; set; }

    IApiResponseMessageRenderer IQueryWithMessageRenderer.MessageRenderer =>
        ProductListRenderer.Default;

    public ApiResponseMessage Execute(HttpClient pimApiClient)
    {
        var id =
            this.CategoryId ?? Program.ReadValue<Guid?>("Please enter category ID:", Guid.Empty);

        var query = new ODataQuery<ProductDto>
        {
            Count = true,
            Top = this.GetTopValue(),
            Skip = this.GetSkipValue(),
            Filter = $"{nameof(ProductDto.CategoryTrees)}/any(c: c/categoryTreeId eq {id})",
            Expand = nameof(ProductDto.CategoryTrees),
            OrderBy = nameof(ProductDto.ProductNumber)
        };

        return pimApiClient.GetAsync(query);
    }
}
