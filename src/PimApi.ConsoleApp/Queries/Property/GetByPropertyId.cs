using PimApi.ConsoleApp.Renderers.Property;
using PimApi.Entities;
using System;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;

namespace PimApi.ConsoleApp.Queries.Property
{
    [Display(
        GroupName = nameof(Property),
        Order = 305,
        Description = "Gets a single property by given Id (Guid)")]
    public class GetByPropertyId : IQuery, IQueryWithMessageRenderer, IQueryWithEntityId
    {
        IApiResponseMessageRenderer IQueryWithMessageRenderer.MessageRenderer => PropertyDetailRenderer.Default;

        public Guid? Id { get; set; }

        public ApiResponseMessage Execute(HttpClient pimApiClient) =>
            pimApiClient.GetAsync(new ODataQuery<PropertyDto>
            {
                Id = this.GetIdValue(),
                Expand = $"{nameof(PropertyDto.PropertyMappings)}($count=true)"
            });
    }
}