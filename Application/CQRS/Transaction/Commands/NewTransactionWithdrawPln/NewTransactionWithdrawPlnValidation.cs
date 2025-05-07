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

namespace Application.CQRS.Transaction.Commands.NewTransactionWithdrawPln;


public class NewTransactionWithdrawPlnValidation : AbstractValidator<NewTransactionWithdrawPlnCommand>
{
    private readonly IAsyncRepository _repository;

    public NewTransactionWithdrawPlnValidation(CurrencySettings currencySettings, IAsyncRepository repository)
    {
        _repository = repository;

        RuleFor(x => x.WalletId).MustAsync(async (m, cancelation) => await IsWalletExists(m)).WithMessage(n => $"Portfel o Id {n.WalletId} nie istnieje");
    }


    private async Task<bool> IsWalletExists(string id) =>
        await (await _repository.AsQueryable<Wallets>())
            .AnyAsync(a => a.Id == id);

}
