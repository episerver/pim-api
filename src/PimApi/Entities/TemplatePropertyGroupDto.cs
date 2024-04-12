namespace PimApi.Entities;

public class TemplatePropertyGroupDto : BaseEntityDtoWithEndpoint
{
    public override string EntityUrlBase => "templatepropertygroups";

    public Guid TemplateId { get; set; }

    public TemplateDto? Template { get; set; }

    public Guid PropertyGroupId { get; set; }

    public PropertyGroupDto? PropertyGroup { get; set; }

    public int DisplaySequence { get; set; }

    public ICollection<TemplatePropertyDto> TemplateProperties { get; set; } = [];
}
