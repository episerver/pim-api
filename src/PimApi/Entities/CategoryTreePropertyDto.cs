using System;

namespace PimApi.Entities
{
    public class CategoryTreePropertyDto : BaseEntityDtoWithEndpoint
    {
        public Guid CategoryTreeId { get; set; }

        public CategoryTreeDto? CategoryTree { get; set; }

        public Guid PropertyId { get; set; }

        public PropertyDto? Property { get; set; }

        public int SortOrder { get; set; }

        public override string EntityUrlBase => "categoryTreeProperties";

    }
}