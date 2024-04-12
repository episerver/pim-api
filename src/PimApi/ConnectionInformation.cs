using static PimApi.Defaults;

namespace PimApi;

public class ConnectionInformation
{
    public string AppKey { get; set; } = string.Empty;

    public string AppSecret { get; set; } = string.Empty;

    public string AuthUrl { get; set; } = authUrlBase;

    public string PimUrlBase { get; set; } = pimUrlBase;

    public void EnsureConnectionInformation()
    {
        if (string.IsNullOrEmpty(this.AuthUrl) || !this.AuthUrl.StartsWith("https://"))
        {
            this.AuthUrl = authUrlBase;
        }

        if (string.IsNullOrEmpty(this.PimUrlBase))
        {
            this.PimUrlBase = pimUrlBase;
        }
    }
}
