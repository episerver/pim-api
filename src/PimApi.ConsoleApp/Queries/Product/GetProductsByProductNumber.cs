using PimApi.ConsoleApp.Renderers.Product;
using PimApi.Entities;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;

namespace PimApi.ConsoleApp.Queries.Product
{
    [Display(
        GroupName = nameof(Product),
        Order = 1,
        Description = "Shows products ordered by productNumber")]
    public class GetProductsByProductNumber : IQuery, IQueryWithMessageRenderer, IQueryWithTopSkip
    {
        public int? Top { get; set; }

        public int? Skip { get; set; }

        public string? Expand { get; set; }

        public string? Filter { get; set; }

        public IApiResponseMessageRenderer MessageRenderer => ProductListRenderer.Default;

        public ApiResponseMessage Execute(HttpClient pimApiClient)
        {
            var query = new ODataQuery<ProductDto>
            {
                Count = true,
                Top = this.GetTopValue(),
                Skip = this.GetSkipValue(),
                Filter = this.Filter,
                Expand = this.Expand,
                OrderBy = nameof(ProductDto.ProductNumber)
            };
            return pimApiClient.GetAsync(query);
        }
    }
}