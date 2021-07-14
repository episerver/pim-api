using PimApi.ConsoleApp.Renderers.Product;
using PimApi.Entities;
using System;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;

namespace PimApi.ConsoleApp.Queries.Product
{
    [Display(
        GroupName = nameof(Product),
        Order = 5,
        Description = "Gets a single product by given Id (Guid)")]
    public class GetByProductId : IQuery, IQueryWithMessageRenderer, IQueryWithEntityId
    {
        public IApiResponseMessageRenderer MessageRenderer => ProductDetailRenderer.Default;

        public Guid? Id { get; set; }

        public ApiResponseMessage Execute(HttpClient pimApiClient) =>
            pimApiClient
                .GetAsync(new ODataQuery<ProductDto>
                {
                    Id = this.GetIdValue(),
                    Expand = $"{nameof(ProductDto.ProductAssets)}($expand={nameof(ProductAssetDto.Asset)}),"
                        + $"{nameof(ProductDto.CategoryTrees)}($expand={nameof(ProductCategoryTreeDto.CategoryTree)}),"
                        + $"{nameof(ProductDto.ProductRelatedProducts)}($expand={nameof(ProductRelatedProductDto.ProductRelationship)},{nameof(ProductRelatedProductDto.RelateProduct)}),"
                        + $"{nameof(ProductDto.ProductRelatedProductsOf)}($expand={nameof(ProductRelatedProductDto.ProductRelationship)},{nameof(ProductRelatedProductDto.RelateProduct)}),"
                });
    }
}