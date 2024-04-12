namespace PimApi;

public class ODataResponseCollection<TEntity> : ODataResponse<ICollection<TEntity>>
    where TEntity : BaseEntityDto
{
    public ODataResponseCollection() => this.Value = [];

    public new ICollection<TEntity> Value
    {
        get => base.Value;
        set => base.Value = value;
    }
}

public class ODataResponse<TEntity> : ODataResponse
    where TEntity : class
{
    public new TEntity Value
    {
        get => (TEntity)base.Value;
        set => base.Value = value;
    }
}

public abstract class ODataResponse
{
    public virtual object Value { get; set; } = null!;

    public int? Count { get; set; }

    public string? Context { get; set; }

    public string? NextLink { get; set; }
}
