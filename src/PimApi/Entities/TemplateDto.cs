using PimApi.Extensions;

namespace PimApi.Entities;

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

    public ICollection<TemplateDto> ChildrenTemplates { get; set; } = [];

    public ICollection<TemplatePropertyGroupDto> TemplatePropertyGroups { get; set; } = [];

    public int TemplatePropertyGroupCount() =>
        this.GetCount(ref templatePropertyGroups, nameof(TemplatePropertyGroups));

    public int ChildrenTemplatesCount() =>
        this.GetCount(ref childrenTemplates, nameof(ChildrenTemplates));
}
