using System;
using System.Collections.Generic;

namespace PimApi.Entities
{
    public class TemplatePropertyGroupDto : BaseEntityDtoWithEndpoint
    {
        public override string EntityUrlBase => "templatepropertygroups";

        public Guid TemplateId { get; set; }

        public TemplateDto? Template { get; set; }

        public Guid PropertyGroupId { get; set; }

        public PropertyGroupDto? PropertyGroup { get; set; }

        public int DisplaySequence { get; set; }

        public System.Collections.Generic.ICollection<TemplatePropertyDto> TemplateProperties { get; set; } = new HashSet<TemplatePropertyDto>();
    }
}