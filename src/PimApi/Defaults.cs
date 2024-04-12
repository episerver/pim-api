using System.Resources;

namespace PimApi;

public static class Defaults
{
    private static readonly ResourceManager resourceManager =
        new("PimApi.Resource", typeof(Defaults).Assembly);

    /// <summary> Used to authenticate AppKey and AppSecret for PIM access</summary>
    internal static readonly string authUrlBase = resourceManager.GetString(nameof(authUrlBase));

    /// <summary>PIM Endpoint</summary>
    internal static readonly string pimUrlBase = resourceManager.GetString(nameof(pimUrlBase));

    public const string oDataContext = "@odata.context";
    public const string oDataCount = "@odata.count";
    public const string oDataNextLink = "@odata.nextLink";
    public const string oDataValue = "value";

    /// <summary>Max number of results to be returned in a single request</summary>
    public const int MaxPageSize = 1_000;
}
