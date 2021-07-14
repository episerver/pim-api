using PimApi.ConsoleApp.Renderers.Category;
using PimApi.Entities;
using System;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;

namespace PimApi.ConsoleApp.Queries.Category
{
    [Display(
        GroupName = nameof(Category),
        Order = 155,
        Description = "Gets a single category taxonomy by given Id (Guid)")]
    public class GetByCategoryTaxonomyId : IQuery, IQueryWithMessageRenderer, IQueryWithEntityId
    {
        IApiResponseMessageRenderer IQueryWithMessageRenderer.MessageRenderer => CategoryTaxonomyDetailRenderer.Default;

        public Guid? Id { get; set; }

        public ApiResponseMessage Execute(HttpClient pimApiClient) =>
            pimApiClient.GetAsync(new ODataQuery<CategoryTaxonomyDto>
            {
                Id = this.GetIdValue(),
                Expand = $"{nameof(CategoryTaxonomyDto.Websites)}($expand={nameof(CategoryTaxonomyWebsiteDto.Website)};$count=true)"
            });
    }
}