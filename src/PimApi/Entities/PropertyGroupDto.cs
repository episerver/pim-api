using PimApi.Extensions;

namespace PimApi.Entities;

public class PropertyGroupDto : BaseEntityDtoWithEndpoint
{
    private int? propertyCount;

    public override string EntityUrlBase => "propertygroups";

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public ICollection<PropertyDto> Properties { get; set; } = [];

    public ICollection<TemplatePropertyGroupDto> TemplatePropertyGroups { get; set; } = [];

    /// <summary>
    /// To get the count of group properties without returning all items and data use the following $expand
    /// <para>$expand=properties($top=1;$select=name;$count=true;$orderby=name)</para>
    /// </summary>
    public int PropertyCount() => this.GetCount(ref propertyCount, nameof(Properties));

    public bool HasPropertyCount() => this.HasCount(nameof(Properties));
}
