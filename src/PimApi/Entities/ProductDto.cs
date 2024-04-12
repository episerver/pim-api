namespace PimApi.Entities;

/// <summary> Base Product DTO, should be extended to capture custom PIM properties</summary>
[DebuggerDisplay("ProductNumber = {ProductNumber}")]
public class ProductDto : BaseEntityDtoWithEndpoint
{
    public override string EntityUrlBase => "products";

    public string ProductNumber { get; set; } = string.Empty;

    public string ProductTitle { get; set; } = string.Empty;

    public string UrlSegment { get; set; } = string.Empty;

    public DateTimeOffset? DeactivateOn { get; set; }

    public string? TemplateName { get; set; }

    public string? PrimaryCategoryName { get; set; }

    public string? ParentVariantProductNumber { get; set; }

    public Guid? TemplateId { get; set; }

    public Guid? PrimaryImageAssetId { get; set; }

    public Guid? PrimaryCategoryTreeId { get; set; }

    public Guid? ParentProductId { get; set; }

    public Guid? DefaultChildProductId { get; set; }

    public string Status { get; set; } = string.Empty;

    public bool OnHold { get; set; }

    public decimal PercentComplete { get; set; }

    public decimal PercentRecommendedComplete { get; set; }

    public int PublishedVersionNumber { get; set; }

    public DateTimeOffset? LastPublishedOn { get; set; }

    public int CurrentVersionNumber { get; set; }

    public string TagList { get; set; } = string.Empty;

    public bool IsVariantParent { get; set; }

    public ICollection<Guid> VariantPropertyIdList { get; set; } = [];

    public ICollection<ProductAssetDto> ProductAssets { get; set; } = [];

    public ICollection<ProductCategoryTreeDto> CategoryTrees { get; set; } = [];

    public ICollection<ProductDto> ChildrenProduct { get; set; } = [];

    public ICollection<ProductRelatedProductDto> ProductRelatedProducts { get; set; } = [];

    public ICollection<ProductRelatedProductDto> ProductRelatedProductsOf { get; set; } = [];
}
