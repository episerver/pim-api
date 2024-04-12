using PimApi.ConsoleApp.Renderers.Asset;

namespace PimApi.ConsoleApp.Queries.Asset;

[Display(
    GroupName = nameof(Asset),
    Order = 205,
    Description = "Gets a single asset by given Id (Guid)"
)]
public class GetByAssetId : IQuery, IQueryWithMessageRenderer, IQueryWithEntityId
{
    IApiResponseMessageRenderer IQueryWithMessageRenderer.MessageRenderer =>
        AssetDetailRenderer.Default;

    public Guid? Id { get; set; }

    public ApiResponseMessage Execute(HttpClient pimApiClient) =>
        pimApiClient.GetAsync(
            new ODataQuery<AssetDto>
            {
                Id = this.GetIdValue(),
                Expand =
                    $"{nameof(AssetDto.ProductAssets)}($expand={nameof(ProductAssetDto.Product)})"
            }
        );
}
