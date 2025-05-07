using Application.Interfaces.Providers;
using Hangfire;
using Hangfire.Storage;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Shared.Settings;

namespace Infrastructure.Providers;
public static class HangfireSetUpProvider
{
    public static void RecurringJobSetUp(IApplicationBuilder app, HangfireSettings hangfireSettings)
    {
        IMonitoringApi monitoringApi = JobStorage.Current.GetMonitoringApi();

        var existedServer = monitoringApi.Servers();
        foreach (var item in existedServer)
            JobStorage.Current.GetConnection().RemoveServer(item.Name);

        var recurringJobs = JobStorage.Current.GetConnection().GetRecurringJobs();
        foreach (var item in recurringJobs)
            RecurringJob.RemoveIfExists(item.Id);

        var currencyRatesProvider = app.ApplicationServices.GetService<ICurrencyRatesProvider>()!;

        if (hangfireSettings.EnabledNbpSync)
            RecurringJob.AddOrUpdate(
                "NbpSync",
                () => currencyRatesProvider.SaveActualRates(), // Ensure the Task is awaited or completed
                hangfireSettings.NbpSyncCronExpression,   // Convert int to string
                new RecurringJobOptions() { TimeZone = TimeZoneInfo.Local });
    }
}
