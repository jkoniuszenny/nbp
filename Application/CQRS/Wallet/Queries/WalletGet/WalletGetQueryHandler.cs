using Application.Interfaces.Providers;
using Application.Interfaces.Repositories;
using Domain.Entities;
using MediatR;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.CQRS.Wallet.Queries.WalletGet;

internal sealed class WalletGetQueryHandler : IRequestHandler<WalletGetQuery, GlobalResponse<IEnumerable<WalletGetOutput>>>
{
    private readonly IAsyncRepository _asyncRepository;

    public WalletGetQueryHandler(
        IAsyncRepository asyncRepository)
    {
        _asyncRepository = asyncRepository;

    }

    public async Task<GlobalResponse<IEnumerable<WalletGetOutput>>> Handle(WalletGetQuery request, CancellationToken cancellationToken)
    {

        var wallets =await( await _asyncRepository.AsQueryable<Wallets>()).
            Where(x => request.WalletName == null || x.Name == request.WalletName)
            .ToListAsync(cancellationToken);


        var result = wallets.Select(x => new WalletGetOutput(
            x.Id,
            x.Name,
            x.Currencies.Select(c=> new WalletGetCurrency(
                c.Name,
                c.Code,
                c.Value
                ))));

        return await GlobalResponse<IEnumerable<WalletGetOutput>>.SuccessAsync(result);
    }
}
