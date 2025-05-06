using Application.Interfaces.Providers.Api;
using Domain.Api;
using Hangfire;
using Microsoft.Extensions.DependencyInjection;
using RestSharp;
using Shared.NLog.Interfaces;
using Shared.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Providers.Api
{
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


        [Queue("nbp")]
        [DisableConcurrentExecution(3600)]
        [AutomaticRetry(Attempts = 3)]
        public async Task<NbpTableB> NbpSync()
        {
            RestRequest restRequest = new($"exchangerates/tables/b?format=json");

            var requestResult = await _restClient.ExecuteAsync<NbpTableB>(restRequest);

            if (requestResult.IsSuccessful && requestResult.Data is { })
                return requestResult.Data;

            _nLogLogger.LogInfo($"Missing result: {requestResult.ErrorMessage} - {requestResult.Content}");

            return new NbpTableB();
        }
    }
}
