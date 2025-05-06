using Application.Interfaces.Providers;
using Application.Interfaces.Providers.Api;
using Application.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Providers;

public class CurrencyRatesProvider: ICurrencyRatesProvider
{
    private readonly INbpApiProvider _nbpApiProvider;

    public CurrencyRatesProvider(
        INbpApiProvider nbpApiProvider,
        IAsyncRepository repository)
    {
        _nbpApiProvider = nbpApiProvider;
    }

    public async Task SaveActualRates()
    {
        var actualNbpTableB = await _nbpApiProvider.NbpSync();
    }
}
