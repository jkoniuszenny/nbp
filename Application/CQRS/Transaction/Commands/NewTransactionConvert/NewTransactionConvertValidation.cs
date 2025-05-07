using Application.Interfaces.Repositories;
using Domain.Entities;
using MongoDB.Driver.Linq;
using Shared.Settings;

namespace Application.CQRS.Transaction.Commands.NewTransactionConvert;


public class NewTransactionConvertValidation : AbstractValidator<NewTransactionConvertCommand>
{
    private readonly IAsyncRepository _repository;

    public NewTransactionConvertValidation(CurrencySettings currencySettings, IAsyncRepository repository)
    {
        _repository = repository;

        RuleFor(x => x.WalletId).MustAsync(async (m, cancelation) => await IsWalletExists(m)).WithMessage(n => $"Portfel o Id {n.WalletId} nie istnieje");

        RuleFor(r => r.CodeTo)
            .Must(m => currencySettings.Codes.Select(s => s.Code).Contains(m, StringComparer.InvariantCultureIgnoreCase))
            .WithMessage(t => $"Przekazany kod waluty: {t.CodeTo} nie znajduje się w słowniku");


        RuleFor(r => r.CodeFrom)
            .Must(m => currencySettings.Codes.Select(s => s.Code).Contains(m, StringComparer.InvariantCultureIgnoreCase))
            .WithMessage(t => $"Przekazany kod waluty: {t.CodeFrom} nie znajduje się w słowniku");

        RuleFor(r => r.CodeFrom)
            .MustAsync(async (model, m, cancelation) => await IsCurrencyInWallet(model.WalletId, model.CodeFrom))
            .WithMessage(t => $"Przekazany kod waluty: {t.CodeFrom} nie jest dostępny w portfelu");

        RuleFor(r => r.Value).MustAsync(async (model, m, cancelation) => !(await IsCurrencyAvailable(model.WalletId, model.CodeFrom, m))).WithMessage(n => $"Zbyt mała dostępna kwota dla stanu waluty {n.CodeFrom}");

    }


    private async Task<bool> IsWalletExists(string id) =>
        await (await _repository.AsQueryable<Wallets>())
            .AnyAsync(a => a.Id == id);

    private async Task<bool> IsCurrencyAvailable(string id, string code, decimal value) =>
        await (await _repository.AsQueryable<Wallets>())
            .AnyAsync(a => a.Id == id && a.Currencies.Any(e => e.Value <= value && e.Code == code));

    private async Task<bool> IsCurrencyInWallet(string id, string code) =>
        await (await _repository.AsQueryable<Wallets>())
            .AnyAsync(a => a.Id == id && a.Currencies.Any(e => e.Code == code));

}
