using Application.Interfaces.Providers.Api;
using Domain.Api;
using RestSharp;
using Shared.NLog.Interfaces;
using Shared.Settings;

namespace Infrastructure.Providers.Api;

public class NbpApiProvider : INbpApiProvider
{
    private readonly RestClient _restClient;
    private readonly INLogLogger _nLogLogger;

    public NbpApiProvider(
        NbpSettings nbpSettings,
        INLogLogger nLogLogger)
    {
        var options = new RestClientOptions(nbpSettings.Url)
        {
            Timeout = TimeSpan.FromSeconds(nbpSettings.Timeout)
        };

        _restClient = new(options);
        _nLogLogger = nLogLogger;
    }

    public async Task<NbpTableB[]> NbpSync()
    {
        RestRequest restRequest = new($"exchangerates/tables/b?format=json");

        var requestResult = await _restClient.ExecuteAsync<NbpTableB[]>(restRequest);

        if (requestResult.IsSuccessful && requestResult.Data is { })
            return requestResult.Data;

        _nLogLogger.LogInfo($"Missing result: {requestResult.ErrorMessage} - {requestResult.Content}");

        return [];
    }
}
