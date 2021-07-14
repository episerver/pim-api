using PimApi.ConsoleApp.Renderers.Property;
using PimApi.Entities;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;

namespace PimApi.ConsoleApp.Queries.Property
{
    /// <summary>
    /// Gets up to first 1,000 property groups with count of properties in each group
    /// </summary>
    [Display(
        GroupName = nameof(Property),
        Order = 350,
        Description = "Shows property groups")]
    public class GetPropertyGroups
        : IQuery, IQueryWithMessageRenderer
    {
        IApiResponseMessageRenderer IQueryWithMessageRenderer.MessageRenderer =>
            PropertyGroupListRenderer.Default;

        public ApiResponseMessage Execute(HttpClient pimApiClient) =>
            pimApiClient.GetAsync(new ODataQuery<PropertyGroupDto>
            {
                Select = $"{nameof(PropertyGroupDto.Name)},{nameof(PropertyGroupDto.Description)}",
                OrderBy = nameof(PropertyGroupDto.Name),
                Expand = $"properties($top=1;$select=name;$count=true;$orderby=name)",
                Count = true
            });
    }
}