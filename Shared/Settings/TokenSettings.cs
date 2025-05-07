namespace Shared.Settings;

public class TokenSettings: ISettings
{
    public string SecretKey { get; set; }
    public string RefreshKey { get; set; }
    public string AudienceKey { get; set; }
    public string IssuerKey { get; set; }
}
