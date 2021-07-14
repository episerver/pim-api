using System;
using System.Collections.Generic;

namespace PimApi.Entities
{
    public class PropertyDto : BaseEntityDtoWithEndpoint
    {
        public override string EntityUrlBase => "properties";

        public string Name { get; set; } = string.Empty;

        public string DisplayName { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public Guid PropertyGroupId { get; set; }

        public PropertyGroupDto? PropertyGroup { get; set; }

        public int DisplaySequence { get; set; }

        public bool IsSystemProperty { get; set; }

        public bool IsGloballyRequired { get; set; }

        public string Type { get; set; } = string.Empty;

        public string ControlType { get; set; } = string.Empty;

        public int StringMinLength { get; set; }

        public int StringMaxLength { get; set; }

        public string StringRegEx { get; set; } = string.Empty;

        public bool StringAllowAdHocValues { get; set; }

        public string BooleanLabel { get; set; } = string.Empty;

        public decimal NumberMinValue { get; set; }

        public decimal NumberMaxValue { get; set; }

        public int NumberOfDecimals { get; set; }

        public string DefaultValue { get; set; } = string.Empty;

        public bool IsExternalManaged { get; set; }

        public bool IsIncludeTemplateSpecificValues { get; set; }

        // todo: will need to change StringValueBag to PropertyValues
        public System.Collections.Generic.ICollection<string> StringValueBag { get; set; } = new HashSet<string>();

        public System.Collections.Generic.ICollection<PropertyMappingDto> PropertyMappings { get; set; } = new HashSet<PropertyMappingDto>();

        public System.Collections.Generic.ICollection<TemplatePropertyDto> TemplateProperties { get; set; } = new HashSet<TemplatePropertyDto>();

        public System.Collections.Generic.ICollection<CategoryTreePropertyDto> CategoryTrees { get; set; } = new HashSet<CategoryTreePropertyDto>();
    }
}