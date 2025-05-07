using Application.CQRS.Wallet.Commands.WalletCreate;
using Application.Interfaces.Providers;
using Application.Interfaces.Repositories;
using Domain.Entities;
using MediatR;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Shared.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.CQRS.Transaction.Commands.NewTransaction;

internal sealed class NewTransactionAddCommandHandler : IRequestHandler<NewTransactionAddCommand, GlobalResponse>
{
    private readonly IAsyncRepository _asyncRepository;
    private readonly IValidator<NewTransactionAddCommand> _validator;
    private readonly CurrencySettings _currencySettings;

    public NewTransactionAddCommandHandler(
        IAsyncRepository asyncRepository,
        IValidator<NewTransactionAddCommand> validator,
        CurrencySettings currencySettings
        )
    {
        _asyncRepository = asyncRepository;
        _validator = validator;
        _currencySettings = currencySettings;
    }

    public async Task<GlobalResponse> Handle(NewTransactionAddCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            return await GlobalResponse.ValidationsAsync(validationResult.Errors.Select(x => x.ErrorMessage));

        var newTransaction = new Transactions
        {
            WalletId = request.WalletId,
            Value = request.Value,
            CurrencyCode = request.Code,
            TransactionType = TransactionType.Add
        };


        var wallet = await (await _asyncRepository.AsQueryable<Wallets>()).
            Where(w => w.Id == request.WalletId)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);

        var addedCurrenceInWallet = wallet.Currencies.FirstOrDefault(c => c.Code == request.Code);

        if (addedCurrenceInWallet is { })
            addedCurrenceInWallet.Value += request.Value;
        else
        {
            var newCurrency = new Currency
            {
                Name = _currencySettings.Codes.First(f => f.Code == request.Code).Name,
                Code = request.Code,
                Value = request.Value
            };

            wallet.Currencies = [.. wallet.Currencies, newCurrency];
        }

        _asyncRepository.ClientSessionHandle.StartTransaction();

        try
        {
            await _asyncRepository.InsertList([newTransaction]);
            await _asyncRepository.UpdateOne(wallet);

            await _asyncRepository.ClientSessionHandle.CommitTransactionAsync(cancellationToken);


        }
        catch
        {
            await _asyncRepository.ClientSessionHandle.AbortTransactionAsync(cancellationToken);
            return await GlobalResponse.FailAsync("Wystąpił błąd podczas przetwarzania transakcji.");
        }


        return await GlobalResponse.SuccessAsync();
    }
}
