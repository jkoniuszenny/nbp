namespace Shared.Settings;

public class ResponseTimeSettings: ISettings
{
    public int MilisecondsElapsedToNotify { get; set; }
    public bool Enabled { get; set; }
}

