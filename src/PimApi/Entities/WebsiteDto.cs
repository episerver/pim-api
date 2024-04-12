namespace PimApi.Entities;

public class WebsiteDto : BaseEntityDtoWithEndpoint
{
    public override string EntityUrlBase => "websites";

    public Guid IscWebsiteId { get; set; }

    public string Name { get; set; } = string.Empty;

    public ICollection<CategoryTaxonomyWebsiteDto> CategoryTaxonomies { get; set; } = [];
}
