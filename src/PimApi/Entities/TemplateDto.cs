using PimApi.Extensions;
using System;
using System.Collections.Generic;

namespace PimApi.Entities
{
    public class TemplateDto : BaseEntityDtoWithEndpoint
    {
        private int? templatePropertyGroups;
        private int? childrenTemplates;

        public override string EntityUrlBase => "templates";

        public string Name { get; set; } = string.Empty;

        public Guid? ParentTemplateId { get; set; }

        public TemplateDto? ParentTemplate { get; set; }

        public bool IsTemplate { get; set; }

        public bool IsStarterTemplate { get; set; }

        public bool IsDeleting { get; set; }

        public bool IsProductImageRequired { get; set; }

        public bool HasChild { get; set; }

        public int DisplaySequence { get; set; }

        public Guid? ProductClassId { get; set; }

        public System.Collections.Generic.ICollection<TemplateDto> ChildrenTemplates { get; set; } = new HashSet<TemplateDto>();

        public System.Collections.Generic.ICollection<TemplatePropertyGroupDto> TemplatePropertyGroups { get; set; } = new HashSet<TemplatePropertyGroupDto>();

        public int TemplatePropertyGroupCount() =>
            this.GetCount(ref templatePropertyGroups, nameof(TemplatePropertyGroups));

        public int ChildrenTemplatesCount() =>
            this.GetCount(ref childrenTemplates, nameof(ChildrenTemplates));
    }
}