namespace Shared.Settings;
public class HangfireSettings
{
    public string Username { get; set; } 
    public string Password { get; set; } 
    public bool Enabled { get; set; }
    public bool EnabledNbpSync { get; set; }
    public int NbpSyncCronExpression { get; set; }
}
