namespace PimApi;

public struct QueryDescriptor
{
    public string DisplayName { get; set; }

    public string Description { get; set; }

    public int QueryNumber { get; set; }

    public string Group { get; set; }

    public IQuery Query { get; set; }

    public readonly bool IsSet() => Query is not null;
}
