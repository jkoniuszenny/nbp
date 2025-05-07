using Application.Interfaces.Providers;
using Application.Interfaces.Providers.Api;
using Application.Interfaces.Repositories;
using Domain.Entities;
using Hangfire;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Providers;

public class CurrencyRatesProvider: ICurrencyRatesProvider
{
    private readonly INbpApiProvider _nbpApiProvider;
    private readonly IAsyncRepository _repository;

    public CurrencyRatesProvider(
        INbpApiProvider nbpApiProvider,
        IAsyncRepository repository)
    {
        _nbpApiProvider = nbpApiProvider;
        _repository = repository;
    }

    [Queue("nbp")]
    [DisableConcurrentExecution(3600)]
    [AutomaticRetry(Attempts = 3)]
    public async Task SaveActualRates()
    {
        var actualNbpTableB = await _nbpApiProvider.NbpSync();

        var isExistedActualTabeleB = await (await _repository.AsQueryable<CurrencyRates>())
            .Where(x => x.No == actualNbpTableB[0].No)
            .AnyAsync();

        if (isExistedActualTabeleB) 
            return;
        
        var mappedResult = actualNbpTableB
            .Select(x =>  new CurrencyRates
            {
                No = x.No,
                EffectiveDate = x.EffectiveDate,
                Rates = [.. x.Rates.Select(s => new Rates
                {
                    Currency = s.Currency,
                    Code = s.Code,
                    Mid = s.Mid
                })]
            })
            .ToList();

        await _repository.InsertList(mappedResult);
    }
}
