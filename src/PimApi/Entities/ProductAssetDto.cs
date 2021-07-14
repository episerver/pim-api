using System;

namespace PimApi.Entities
{
    public class ProductAssetDto : BaseEntityDtoWithEndpoint
    {
        public override string EntityUrlBase => "productassets";

        public Guid AssetId { get; set; }

        public AssetDto? Asset{ get; set; }

        public Guid ProductId { get; set; }

        public ProductDto? Product { get; set; }
    }
}