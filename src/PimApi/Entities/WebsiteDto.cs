using System;
using System.Collections.Generic;

namespace PimApi.Entities
{
    public class WebsiteDto : BaseEntityDtoWithEndpoint
    {
        public override string EntityUrlBase => "websites";

        public Guid IscWebsiteId { get; set; }

        public string Name { get; set; } = string.Empty;

        public System.Collections.Generic.ICollection<CategoryTaxonomyWebsiteDto> CategoryTaxonomies { get; set; } = new HashSet<CategoryTaxonomyWebsiteDto>();
    }
}