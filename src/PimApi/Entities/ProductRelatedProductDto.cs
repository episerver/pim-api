namespace PimApi.Entities;

public class ProductRelatedProductDto : BaseEntityDto
{
    public Guid ProductId { get; set; }

    public ProductDto? Product { get; set; }

    public Guid ProductRelationshipId { get; set; }

    public ProductRelationshipDto? ProductRelationship { get; set; }

    public Guid RelatedProductId { get; set; }

    public ProductDto? RelateProduct { get; set; }

    public int SortOrder { get; set; }
}
