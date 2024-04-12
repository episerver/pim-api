namespace PimApi.Entities;

public class GenericEntityDto : BaseEntityDto { }

/// <summary>Represents entity data stored in PIM DB table without an Http endpoint<para>Must be loaded using $expand</para> </summary>
public abstract class BaseEntityDto
{
    public Guid Id { get; set; }

    public string CreatedBy { get; set; } = string.Empty;

    public DateTimeOffset CreatedOn { get; set; }

    public DateTimeOffset ModifiedOn { get; set; }

    public string ModifiedBy { get; set; } = string.Empty;

    /// <summary>Stores values that don't have a property assigned</summary>
    public IDictionary<string, object?> PropertyBag { get; } =
        new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
}
