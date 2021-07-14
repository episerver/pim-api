using System;
using System.Collections.Generic;

namespace PimApi.Entities
{
    public class TemplatePropertyDto : BaseEntityDtoWithEndpoint
    {
        public override string EntityUrlBase => "templateproperties";

        public Guid TemplateId { get; set; }

        public TemplateDto? Template { get; set; }

        public Guid TemplatePropertyGroupId { get; set; }

        public TemplatePropertyGroupDto? TemplatePropertyGroup { get; set; }

        public Guid PropertyId { get; set; }

        public PropertyDto? Property { get; set; }

        public int DisplaySequence { get; set; }

        public bool IsRequired { get; set; }

        public bool IsRecommended { get; set; }

        public System.Collections.Generic.ICollection<string>? StringValueBag { get; set; } = new HashSet<string>();
    }
}