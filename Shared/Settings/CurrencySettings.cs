namespace Shared.Settings;
public class CurrencySettings: ISettings
{
    public Codes[] Codes { get; set; } 

}

public class Codes
{
    public string Name { get; set; }
    public string Code { get; set; }
}