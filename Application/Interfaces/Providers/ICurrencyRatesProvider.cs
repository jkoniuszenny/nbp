namespace Application.Interfaces.Providers;

public interface ICurrencyRatesProvider : IProvider
{
    Task SaveActualRates();
}
