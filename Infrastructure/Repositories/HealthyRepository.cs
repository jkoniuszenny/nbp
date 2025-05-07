using Application.Interfaces.Repositories;
using Infrastructure.Database;
using Shared.Enums;

namespace Infrastructure.Repositories;

public class HealthyRepository : IHealthyRepository
{
    private readonly DatabaseMongoContext _databaseContext;

    public HealthyRepository(
        DatabaseMongoContext databaseContext)
    {
        _databaseContext = databaseContext;
    }

    public async Task<string> GetHealthyAsync()
    {
        return
            await _databaseContext.CheckHealthAsync()
            ? nameof(DatabaseHealthy.Healthy)
            : nameof(DatabaseHealthy.Unhealthy);
    }
}
