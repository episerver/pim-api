namespace PimApi.Entities;

public class CategoryTaxonomyWebsiteDto : BaseEntityDto
{
    public Guid CategoryTaxonomyId { get; set; }

    public CategoryTaxonomyDto? CategoryTaxonomy { get; set; }

    public Guid WebsiteId { get; set; }

    public WebsiteDto? Website { get; set; }
}
