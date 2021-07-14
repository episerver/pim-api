using PimApi.ConsoleApp.Renderers.Property;
using PimApi.Entities;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;

namespace PimApi.ConsoleApp.Queries.Property
{
    [Display(
        GroupName = nameof(Property),
        Order = 310,
        Description = "Searches properties by control type")]
    public class SearchPropertiesByControlType : IQuery, IQueryWithMessageRenderer, IQueryWithTopSkip
    {
        public int? Top { get; set; }

        public int? Skip { get; set; }

        public string? ControlType { get; set; }

        IApiResponseMessageRenderer IQueryWithMessageRenderer.MessageRenderer => PropertyListRenderer.Default;

        public ApiResponseMessage Execute(HttpClient pimApiClient)
        {
            var controlType = this.ControlType 
                ?? Program.ReadValue("Please enter the control type", "textfield");

            return pimApiClient.GetAsync(new ODataQuery<PropertyDto>
            {
                Count = true,
                Top = this.GetTopValue(),
                Skip = this.GetSkipValue(),
                OrderBy = nameof(PropertyDto.DisplaySequence),
                Filter = $"{nameof(PropertyDto.ControlType)} eq '{controlType}'"
            });
        }
    }
}