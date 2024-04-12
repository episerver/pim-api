namespace PimApi;

[DebuggerDisplay("{DebugDisplay}")]
public class ODataQuery<TEntity>
    where TEntity : BaseEntityDtoWithEndpoint, new()
{
    private readonly TEntity entity = new();

    /// <summary>Gets the number of records matching query</summary>
    public bool? Count { get; set; }

    /// <summary>Gets records matching a filter such as name = value</summary>
    public string? Filter { get; set; }

    /// <summary>Includes navigation properties</summary>
    public string? Expand { get; set; }

    /// <summary>Creates $top in request <para>When set responses will not return NextLink</para></summary>
    public int? Top { get; set; }

    /// <summary>Skips number of records in matched query <para>Used for pagination</para></summary>
    public int? Skip { get; set; }

    /// <summary>Orders results by field, for descending order  <para>For descending order: nameOfProperty desc</para></summary>
    public string? OrderBy { get; set; }

    /// <summary>Only returns selected properties, comma delimited</summary>
    public string? Select { get; set; }

    /// <summary>Gets a single entity with given Id</summary>
    public Guid? Id { get; set; }

    private string DebugDisplay => this.ToApiRequestString();

    public string ToApiRequestString()
    {
        if (this.Id is not null)
        {
            var url = $"{entity.EntityUrlBase}({Id.Value})";

            if (this.Expand is string)
            {
                url += $"?$expand={this.Expand}";
            };

            return url;
        }

        var request = new StringBuilder(250);
        request.Append(entity.EntityUrlBase);
        request.Append('?');

        foreach (var p in this.GetType().GetProperties())
        {
            var value = p.GetValue(this);
            if (value is null) { continue; }

            if (value is bool b)
            {
                value = b ? "true" : "false";
            }

            request.Append($"${p.Name.ToLower()}={value}&");
        }

        return request.ToString().TrimEnd('&');
    }

    public override string ToString() => this.ToApiRequestString();

    public static implicit operator string(ODataQuery<TEntity> oDataQuery) =>
        oDataQuery.ToApiRequestString();
}