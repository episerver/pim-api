using PimApi.ConsoleApp.Renderers.Asset;
using PimApi.Extensions;

namespace PimApi.ConsoleApp.Queries.Asset;

[Display(
    GroupName = nameof(Asset),
    Order = 200,
    Description = "Shows assets filtered by asset type and ordered by Name"
)]
public class GetAssets : IQuery, IQueryWithMessageRenderer, IQueryWithTopSkip
{
    private readonly string[] types = new[] { "image", "other" };

    IApiResponseMessageRenderer IQueryWithMessageRenderer.MessageRenderer =>
        AssetListRenderer.Default;

    public bool? ImageFilter { get; set; }

    public bool? ExternalOnly { get; set; }

    public int? Skip { get; set; }

    public int? Top { get; set; }

    public ApiResponseMessage Execute(HttpClient pimApiClient)
    {
        var imageFilter = this.ImageFilter;
        var externalOnly = this.ExternalOnly;

        if (imageFilter is null)
        {
            imageFilter = Program
                .ReadValue("Please enter asset type:", "image", types)
                .StartsWith("i", StringComparison.OrdinalIgnoreCase);
        }

        if (externalOnly is null)
        {
            externalOnly = Program
                .ReadValue("External assets only?", "no", Program.YesNoChoice)
                .IsYes();
        }

        var query = new ODataQuery<AssetDto>
        {
            Count = true,
            OrderBy = nameof(AssetDto.Name),
            Top = this.GetTopValue(),
            Skip = this.GetSkipValue(),
            Filter =
                $"assettype eq '{GetImageFilter(imageFilter)}' and externalfileurl {GetExternalOnly(externalOnly)}"
        };

        return pimApiClient.GetAsync(query);
    }

    private static string GetImageFilter(bool? imageFilter) =>
        imageFilter is true ? "image" : "other";

    private static string GetExternalOnly(bool? externalOnly) =>
        externalOnly is true ? "ne ''" : "eq ''";
}
