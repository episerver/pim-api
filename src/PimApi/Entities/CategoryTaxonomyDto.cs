namespace PimApi.Entities;

public class CategoryTaxonomyDto : BaseEntityDtoWithEndpoint
{
    public override string EntityUrlBase => "categoryTaxonomies";

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public ICollection<CategoryTaxonomyWebsiteDto> Websites { get; set; } = [];
}
