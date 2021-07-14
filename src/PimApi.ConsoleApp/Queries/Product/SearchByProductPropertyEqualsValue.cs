using PimApi.ConsoleApp.Renderers.Product;
using PimApi.Entities;
using PimApi.Extensions;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;

namespace PimApi.ConsoleApp.Queries.Product
{
    [Display(
        GroupName = nameof(Product),
        Order = 11,
        Description = "Shows filtering property by value (ex: by status or product number)")]
    public class SearchByProductPropertyEqualsValue : IQuery, IQueryWithMessageRenderer, IQueryWithTopSkip
    {
        public IApiResponseMessageRenderer MessageRenderer => ProductListRenderer.Default;

        public string? ValueToSearch { get; set; }

        public string? PropertyToSearch { get; set; }

        public int? Top { get; set; }

        public int? Skip { get; set; }

        public bool IsNotEqualsFilter { get; set; }

        public ApiResponseMessage Execute(HttpClient pimApiClient)
        {
            var valueToSearch = this.ValueToSearch;
            var propertyToSearch = this.PropertyToSearch;

            propertyToSearch ??= Program.ReadValue(
                "Please enter product property to search:",
                "status");

            valueToSearch ??= Program.ReadValue(
                "Please enter product property value:",
                "published");

            var query = new ODataQuery<ProductDto>
            {
                Top = this.GetTopValue(),
                Skip = this.GetSkipValue(),
                OrderBy = nameof(ProductDto.ProductNumber),
                Filter = $"{propertyToSearch} {(this.IsNotEqualsFilter ? "ne" : "eq")} {valueToSearch.GetValueToSearch()}"
            };

            return pimApiClient.GetAsync(query);
        }
    }
}