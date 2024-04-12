using PimApi.Extensions;

namespace PimApi.Entities;

public class CategoryTreeDto : BaseEntityDtoWithEndpoint
{
    private int? childCount;
    private int? productCount;
    private int? propertyCount;

    public override string EntityUrlBase => "categorytrees";

    public string Name { get; set; } = string.Empty;

    public bool AutoAssignAttributes { get; set; }

    public Guid CategoryTaxonomyId { get; set; }

    public CategoryTaxonomyDto? CategoryTaxonomy { get; set; }

    public string Description { get; set; } = string.Empty;

    public string UrlSegment { get; set; } = string.Empty;

    public DateTime ActivateOn { get; set; }

    public DateTime? DeactivateOn { get; set; }

    public Guid? ParentId { get; set; }

    public CategoryTreeDto? Parent { get; set; }

    public int DisplaySequence { get; set; }

    public Guid? SmallImageAssetId { get; set; }

    public AssetDto? SmallImageAsset { get; set; }

    public Guid? LargeImageAssetId { get; set; }

    public AssetDto? LargeImageAsset { get; set; }

    public bool IsDynamic { get; set; } = false;

    public bool ExcludedFromDynamicRecommendations { get; set; } = false;

    public bool NeedsPublishing { get; set; }

    public string? DynamicFilter { get; set; }

    public bool HasChild { get; set; }

    public string? PageTitle { get; set; }

    public string? MetaDescription { get; set; }

    public string? MetaKeywords { get; set; }

    public AssetDto? OpenGraphImage { get; set; }

    public Guid? OpenGraphImageId { get; set; }

    public string? OpenGraphTitle { get; set; }

    public string? OpenGraphUrl { get; set; }

    public ICollection<CategoryTreeDto> ChildCategoryTrees { get; set; } = [];

    public ICollection<ProductCategoryTreeDto> Products { get; set; } = [];

    public ICollection<CategoryTreePropertyDto> Properties { get; set; } = [];

    /// <summary>
    /// To get the count of group properties without returning all items and data use the following $expand
    /// <para>$expand=childCategoryTrees($top=1;$select=id;$count=true;$orderby=id)</para>
    /// </summary>
    /// <returns></returns>
    public int ChildCategoryCount() => this.GetCount(ref childCount, nameof(ChildCategoryTrees));

    public int ProductCount() => this.GetCount(ref productCount, nameof(Products));

    public int PropertyCount() => this.GetCount(ref propertyCount, nameof(Properties));
}
