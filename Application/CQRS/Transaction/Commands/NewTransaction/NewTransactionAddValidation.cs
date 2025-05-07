using Application.Interfaces.Repositories;
using Domain.Entities;
using FluentValidation;
using MongoDB.Driver.Linq;
using Shared.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.CQRS.Transaction.Commands.NewTransaction;

public class NewTransactionAddValidation : AbstractValidator<NewTransactionAddCommand>
{
    private readonly IAsyncRepository _repository;

    public NewTransactionAddValidation(CurrencySettings currencySettings, IAsyncRepository repository)
    {
        _repository = repository;

        RuleFor(x => x.WalletId).MustAsync(async (m, cancelation) => await IsWalletExists(m)).WithMessage(n => $"Portfel o Id {n.WalletId} nie istnieje");

        RuleFor(r => r.Code)
            .Must(m => currencySettings.Codes.Select(s => s.Code).Contains(m, StringComparer.InvariantCultureIgnoreCase))
            .WithMessage(t => $"Przekazany kod waluty: {t.Code} nie znajduje się w słowniku");

        RuleFor(r => r.Value).GreaterThan(0).WithMessage("Wartość powinna być większe niż 0");
    }


    private async Task<bool> IsWalletExists(string id) =>
        await (await _repository.AsQueryable<Wallets>())
            .AnyAsync(a => a.Id == id);
}
