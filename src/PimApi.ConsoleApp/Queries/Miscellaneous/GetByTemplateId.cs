using PimApi.ConsoleApp.Renderers.Miscellaneous;
using PimApi.Entities;
using System;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;

namespace PimApi.ConsoleApp.Queries.Miscellaneous
{
    [Display(
        GroupName = nameof(Miscellaneous),
        Order = 915,
        Description = "Gets a single template by given Id (Guid)")]
    public class GetByTemplateId : IQuery, IQueryWithMessageRenderer, IQueryWithEntityId
    {
        public IApiResponseMessageRenderer MessageRenderer => TemplateDetailRenderer.Default;

        public Guid? Id { get; set; }

        public ApiResponseMessage Execute(HttpClient pimApiClient) =>
            pimApiClient
                .GetAsync(new ODataQuery<TemplateDto>
                {
                    Id = this.GetIdValue(),
                    Expand = $"{nameof(TemplateDto.TemplatePropertyGroups)}($count=true)"
                });
    }
}