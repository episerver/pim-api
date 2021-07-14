using PimApi.ConsoleApp.Renderers.Asset;
using PimApi.Entities;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;

namespace PimApi.ConsoleApp.Queries.Asset
{
    [Display(
        GroupName = nameof(Asset),
        Order = 210,
        Description = "Shows assets filtered by file extension and ordered by Name")]
    public class GetAssetsByFileExtension : IQuery, IQueryWithMessageRenderer, IQueryWithTopSkip
    {
        public string? FileExtension { get; set; }

        public int? Top { get; set; }

        public int? Skip { get; set; }

        IApiResponseMessageRenderer IQueryWithMessageRenderer.MessageRenderer => AssetListRenderer.Default;

        public ApiResponseMessage Execute(HttpClient pimApiClient)
        {
            var fileExtension = this.FileExtension;

            if (fileExtension is null)
            {
                fileExtension = Program.ReadValue(
                    "Please enter asset type file extension (default = jpg)?",
                    "jpg");
            }

            var query = new ODataQuery<AssetDto>
            {
                Count = true,
                OrderBy = nameof(AssetDto.Name),
                Top = this.GetTopValue(),
                Skip = this.GetSkipValue(),
                Filter = $"fileextension eq '{fileExtension.TrimStart('.')}'"
            };

            return pimApiClient.GetAsync(query);
        }
    }
}