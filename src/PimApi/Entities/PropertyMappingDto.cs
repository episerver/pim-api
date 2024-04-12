namespace PimApi.Entities;

public class PropertyMappingDto : BaseEntityDtoWithEndpoint
{
    public override string EntityUrlBase => "propertyMappings";

    public Guid PropertyId { get; set; }

    public PropertyDto? Property { get; set; }

    public string MappedToIscField { get; set; } = string.Empty;

    public bool IscSpecialFieldType { get; set; }

    public string SpecificationName { get; set; } = string.Empty;

    public int SpecificationSortOrder { get; set; } = 0;

    public string SpecificationDescription { get; set; } = string.Empty;

    public bool AttributeIsFilter { get; set; }

    public bool AttributeIsComparable { get; set; }

    public bool AttributeIncludeOnProduct { get; set; }

    public bool AttributeIsSearchable { get; set; }
}
