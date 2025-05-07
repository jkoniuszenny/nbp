using Application.CQRS.Transaction.Commands.NewTransactionSubtract;
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

namespace Application.CQRS.Transaction.Commands.NewTransactionWithdrawPln;

internal sealed class NewTransactionWithdrawPlnCommandHandler : IRequestHandler<NewTransactionWithdrawPlnCommand, GlobalResponse>
{
    private readonly IAsyncRepository _asyncRepository;
    private readonly IValidator<NewTransactionWithdrawPlnCommand> _validator;

    public NewTransactionWithdrawPlnCommandHandler(
        IAsyncRepository asyncRepository,
        IValidator<NewTransactionWithdrawPlnCommand> validator)
    {
        _asyncRepository = asyncRepository;
        _validator = validator;
    }

    public async Task<GlobalResponse> Handle(NewTransactionWithdrawPlnCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            return await GlobalResponse.ValidationsAsync(validationResult.Errors.Select(x => x.ErrorMessage));

        var wallet = await (await _asyncRepository.AsQueryable<Wallets>())
            .Where(w => w.Id == request.WalletId)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);

        var walletCurrencyCodes = wallet.Currencies.Select(s => s.Code);

        var actualCurrencyRates = await (await _asyncRepository.AsQueryable<CurrencyRates>())
            .OrderByDescending(c => c.EffectiveDate)
            .Take(1)
            .SelectMany(s => s.Rates)
            .Where(c => walletCurrencyCodes.Contains(c.Code))
            .Select(p => new { p.Code, p.Mid })
            .ToListAsync(cancellationToken: cancellationToken);

        var availiblePln = wallet.Currencies.Sum(c => c.Value * actualCurrencyRates.FirstOrDefault(f => f.Code == c.Code)?.Mid);

        if(request.Value > availiblePln)
            return await GlobalResponse.ValidationsAsync(["Zbyt mała dostępna kwota w PLN"]);

        return await GlobalResponse.SuccessAsync();
    }
}
