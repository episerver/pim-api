namespace PimApi.Entities;

public class AssetDto : BaseEntityDtoWithEndpoint
{
    public override string EntityUrlBase => "assets";

    public string Name { get; set; } = string.Empty;

    public Guid? AssetTreeId { get; set; }

    public string AssetType { get; set; } = string.Empty;

    public string FileExtension { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Type { get; set; } = string.Empty;

    public string? AssetFileName { get; set; }

    public string TagList { get; set; } = string.Empty;

    public string AltText { get; set; } = string.Empty;

    public string DocumentLanguage { get; set; } = string.Empty;

    public int? ProductCount { get; set; }

    public int? CategoryCount { get; set; }

    public string? CurrentAssetHash { get; set; }

    public bool IsExternal { get; set; }

    public string? InternalFileName { get; set; }

    public string? UrlLarge { get; set; }

    public string? UrlMedium { get; set; }

    public string? UrlSmall { get; set; }

    public string ExternalFileUrl { get; set; } = string.Empty;

    public bool IsArchived { get; set; }

    public int? CurrentVersionNumber { get; set; } = 0;

    public string? PreviousInternalFileName { get; set; }

    public string PreviousFileExtension { get; set; } = string.Empty;

    public System.Collections.Generic.ICollection<ProductAssetDto> ProductAssets { get; set; } = new HashSet<ProductAssetDto>();
}