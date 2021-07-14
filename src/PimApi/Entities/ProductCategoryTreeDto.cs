using System;

namespace PimApi.Entities
{
    public class ProductCategoryTreeDto : BaseEntityDtoWithEndpoint
    {
        public override string EntityUrlBase => "categoryTreeProducts";

        public Guid CategoryTreeId { get; set; }

        public CategoryTreeDto? CategoryTree { get; set; }

        public Guid ProductId { get; set; }

        public ProductDto? Product { get; set; }
    }
}