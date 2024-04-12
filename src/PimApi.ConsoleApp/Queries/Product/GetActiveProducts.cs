using PimApi.ConsoleApp.Renderers.Product;

namespace PimApi.ConsoleApp.Queries.Product;

[Display(
    GroupName = nameof(Product),
    Order = 2,
    Description = "Shows active products filtering by on deactiveOn"
)]
public class GetActiveProducts : IQuery, IQueryWithMessageRenderer, IQueryWithTopSkip
{
    public int? Top { get; set; }

    public int? Skip { get; set; }

    public IApiResponseMessageRenderer MessageRenderer => ProductListRenderer.Default;

    public ApiResponseMessage Execute(HttpClient pimApiClient)
    {
        var request = new ODataQuery<ProductDto>
        {
            Top = this.GetTopValue(),
            Skip = this.GetSkipValue(),
            Count = true,
            OrderBy = nameof(ProductDto.ProductNumber),
            Filter = "deactivateOn eq null"
        };

        return pimApiClient.GetAsync(request);
    }
}
