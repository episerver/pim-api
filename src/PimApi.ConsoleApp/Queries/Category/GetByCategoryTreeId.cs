using PimApi.ConsoleApp.Renderers.Category;
using PimApi.Entities;
using System;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;

namespace PimApi.ConsoleApp.Queries.Category
{
    [Display(
        GroupName = nameof(Category),
        Order = 105,
        Description = "Gets a single category tree by given Id (Guid)")]
    public class GetByCategoryTreeId : IQuery, IQueryWithMessageRenderer, IQueryWithEntityId
    {
        IApiResponseMessageRenderer IQueryWithMessageRenderer.MessageRenderer => CategoryTreeDetailRenderer.Default;

        public Guid? Id { get; set; }

        public ApiResponseMessage Execute(HttpClient pimApiClient) =>
            pimApiClient.GetAsync(new ODataQuery<CategoryTreeDto>
            {
                Id = this.GetIdValue(),
                Expand = $"{nameof(CategoryTreeDto.Products)}($expand={nameof(ProductCategoryTreeDto.Product)};$count=true),"
                    + $"{nameof(CategoryTreeDto.ChildCategoryTrees)}($count=true;$expand={nameof(CategoryTreeDto.Products)}($count=true;$top=1),{nameof(CategoryTreeDto.ChildCategoryTrees)}($count=true;$top=1))"
            });
    }
}