namespace Shared.Settings;

public class DatabaseMongoSettings :  ISettings
{
    public string ConnectionString { get; set; }
    public string DatabaseName { get; set; }
}
