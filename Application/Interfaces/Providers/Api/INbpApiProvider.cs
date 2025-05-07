using Domain.Api;

namespace Application.Interfaces.Providers.Api;

public interface INbpApiProvider : IProvider
{
    Task<NbpTableB[]> NbpSync();
}
