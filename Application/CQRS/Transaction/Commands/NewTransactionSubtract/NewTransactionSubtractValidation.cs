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

namespace Application.CQRS.Transaction.Commands.NewTransactionSubtract;

public class NewTransactionSubtractValidation : AbstractValidator<NewTransactionSubtractCommand>
{
    private readonly IAsyncRepository _repository;

    public NewTransactionSubtractValidation(CurrencySettings currencySettings, IAsyncRepository repository)
    {
        _repository = repository;

        RuleFor(x => x.WalletId).MustAsync(async (m, cancelation) => await IsWalletExists(m)).WithMessage(n => $"Portfel o Id {n.WalletId} nie istnieje");

        RuleFor(r => r.Code)
          .Must(m => currencySettings.Codes.Select(s => s.Code).Contains(m, StringComparer.InvariantCultureIgnoreCase))
          .WithMessage(t => $"Przekazany kod waluty: {t.Code} nie znajduje się w słowniku");

        RuleFor(r => r.Code)
            .MustAsync(async (model, m, cancelation) => await IsCurrencyInWallet(model.WalletId, model.Code))
            .WithMessage(t => $"Przekazany kod waluty: {t.Code} nie jest dostępny w portfelu");

        RuleFor(r => r.Value).MustAsync(async (model, m, cancelation) =>!(await IsCurrencyAvailable(model.WalletId, model.Code, m))).WithMessage(n => $"Zbyt mała dostępna kwota dla stanu waluty {n.Code}");
    }


    private async Task<bool> IsWalletExists(string id) =>
        await (await _repository.AsQueryable<Wallets>())
            .AnyAsync(a => a.Id == id);

    private async Task<bool> IsCurrencyAvailable(string id, string code, double value) =>
        await (await _repository.AsQueryable<Wallets>())
            .AnyAsync(a => a.Id == id && a.Currencies.Any(e => e.Value <= value && e.Code == code));

    private async Task<bool> IsCurrencyInWallet(string id, string code) =>
        await (await _repository.AsQueryable<Wallets>())
            .AnyAsync(a => a.Id == id && a.Currencies.Any(e => e.Code == code));

}
