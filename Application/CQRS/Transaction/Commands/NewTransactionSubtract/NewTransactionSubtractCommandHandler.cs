using Application.CQRS.Transaction.Commands.NewTransaction;
using Application.Interfaces.Providers;
using Application.Interfaces.Repositories;
using Domain.Entities;
using MediatR;
using MongoDB.Driver.Linq;
using Shared.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.CQRS.Transaction.Commands.NewTransactionSubtract;

internal sealed class NewTransactionSubtractCommandHandler : IRequestHandler<NewTransactionSubtractCommand, GlobalResponse>
{
    private readonly IAsyncRepository _asyncRepository;
    private readonly IValidator<NewTransactionSubtractCommand> _validator;

    public NewTransactionSubtractCommandHandler(
         IAsyncRepository asyncRepository,
         IValidator<NewTransactionSubtractCommand> validator)
    {
        _asyncRepository = asyncRepository;
        _validator = validator;
    }

    public async Task<GlobalResponse> Handle(NewTransactionSubtractCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            return await GlobalResponse.ValidationsAsync(validationResult.Errors.Select(x => x.ErrorMessage));

        var newTransaction = new Transactions
        {
            WalletId = request.WalletId,
            Value = request.Value,
            CurrencyCode = request.Code,
            TransactionType = TransactionType.Subtract
        };


        var wallet = await (await _asyncRepository.AsQueryable<Wallets>()).
            Where(w => w.Id == request.WalletId)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);

        var substructCurrenceInWallet = wallet.Currencies.FirstOrDefault(c => c.Code == request.Code);
        substructCurrenceInWallet!.Value -= request.Value;


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
