using PimApi.ConsoleApp.Renderers.Miscellaneous;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;

namespace PimApi.ConsoleApp.Queries.Miscellaneous
{
    [Display(
        GroupName = nameof(Miscellaneous),
        Order = 999,
        Description = "Shows response for a given OData query")]
    public class GenericQuery : IQuery, IQueryWithMessageRenderer
    {
        public GenericQuery() : this(null) { }

        public GenericQuery(IApiResponseMessageRenderer? apiResponseMessageRenderer)
        {
            this.MessageRenderer = apiResponseMessageRenderer
                ?? new UserChoiceRenderer(this.GetRenderChoices());
        }

        public IApiResponseMessageRenderer MessageRenderer { get; }
        
        public string? QueryText { get; set; }

        public ApiResponseMessage Execute(HttpClient pimApiClient)
        {
            var queryText = this.QueryText;

            if (queryText is null)
            {
                queryText = Program.ReadValue(
                    "Please enter the URL query",
                    "products?$top=5&$select=id,productnumber,producttitle,status,modifiedby,modifiedon");
            }

            return pimApiClient.GetAsync(queryText);
        }
    }
}
