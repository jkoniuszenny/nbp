using Domain.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Providers.Api;

public interface INbpApiProvider : IProvider
{
    Task<NbpTableB[]> NbpSync();
}
