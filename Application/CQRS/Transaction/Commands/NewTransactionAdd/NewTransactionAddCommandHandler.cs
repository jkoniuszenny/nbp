using Application.Interfaces.Repositories;
using Domain.Entities;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Shared.Settings;

namespace Application.CQRS.Transaction.Commands.NewTransactionAdd;

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


        var roundedValue = Math.Round(
            request.Value,
            2,
            MidpointRounding.ToEven);

        var newTransaction = new Transactions
        {
            WalletId = request.WalletId,
            Value = roundedValue,
            CurrencyCodeTo = request.Code,
            TransactionType = TransactionType.Add
        };


        var wallet = await (await _asyncRepository.AsQueryable<Wallets>()).
            Where(w => w.Id == request.WalletId)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);

        wallet.AddCurrency(new Currency
        {
            Name = _currencySettings.Codes.First(f => f.Code == request.Code).Name,
            Code = request.Code,
            Value = roundedValue
        });

       
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
