namespace PimApi;

public class DateTimeProvider
{
    public static readonly DateTimeProvider Default = new();

    public virtual DateTimeOffset GetUtcNow() => DateTimeOffset.UtcNow;
}
