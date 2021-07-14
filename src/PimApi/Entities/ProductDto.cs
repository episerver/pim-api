using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace PimApi.Entities
{
    /// <summary>
    /// Base Product DTO, should be extended to capture custom PIM properties
    /// </summary>
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

        public System.Collections.Generic.ICollection<Guid> VariantPropertyIdList { get; set; } = new HashSet<Guid>();

        public System.Collections.Generic.ICollection<ProductAssetDto> ProductAssets { get; set; } = new HashSet<ProductAssetDto>();

        public System.Collections.Generic.ICollection<ProductCategoryTreeDto> CategoryTrees { get; set; } = new HashSet<ProductCategoryTreeDto>();

        public System.Collections.Generic.ICollection<ProductDto> ChildrenProduct { get; set; } = new HashSet<ProductDto>();

        public System.Collections.Generic.ICollection<ProductRelatedProductDto> ProductRelatedProducts { get; set; } = new HashSet<ProductRelatedProductDto>();

        public System.Collections.Generic.ICollection<ProductRelatedProductDto> ProductRelatedProductsOf { get; set; } = new HashSet<ProductRelatedProductDto>();
    }
}