using Application.Interfaces.Repositories;
using Domain.Entities;
using MongoDB.Driver.Linq;
using Shared.Settings;

namespace Application.CQRS.Transaction.Commands.NewTransactionConvert;

internal sealed class NewTransactionConvertCommandHandler : IRequestHandler<NewTransactionConvertCommand, GlobalResponse>
{
    private readonly IAsyncRepository _asyncRepository;
    private readonly IValidator<NewTransactionConvertCommand> _validator;
    private readonly CurrencySettings _currencySettings;

    public NewTransactionConvertCommandHandler(
        IAsyncRepository asyncRepository,
        IValidator<NewTransactionConvertCommand> validator,
        CurrencySettings currencySettings)
    {
        _asyncRepository = asyncRepository;
        _validator = validator;
        _currencySettings = currencySettings;
    }

    public async Task<GlobalResponse> Handle(NewTransactionConvertCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            return await GlobalResponse.ValidationsAsync(validationResult.Errors.Select(x => x.ErrorMessage));

        var wallet = await (await _asyncRepository.AsQueryable<Wallets>())
            .Where(w => w.Id == request.WalletId)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);


        var actualCurrencyRates = await (await _asyncRepository.AsQueryable<CurrencyRates>())
            .OrderByDescending(c => c.EffectiveDate)
            .Take(1)
            .SelectMany(s => s.Rates)
            .Where(c => c.Code == request.CodeFrom || c.Code == request.CodeTo)
            .Select(p => new { p.Code, p.Mid })
            .ToListAsync(cancellationToken: cancellationToken);


        var convertedValue = Math.Round(
            (request.Value * actualCurrencyRates.FirstOrDefault(c => c.Code == request.CodeFrom)?.Mid ?? 0)
            / actualCurrencyRates.FirstOrDefault(c => c.Code == request.CodeTo)?.Mid ?? 0, 
            2, 
            MidpointRounding.ToEven);

        wallet.SubtractCurrency(new Currency
        {
            Code = request.CodeFrom,
            Value = request.Value
        });

        wallet.AddCurrency(new Currency
        {
            Name = _currencySettings.Codes.First(f => f.Code == request.CodeTo).Name,
            Code = request.CodeTo,
            Value = convertedValue
        });

        var newTransaction = new Transactions
        {
            WalletId = request.WalletId,
            Value = request.Value,
            CurrencyCodeFrom = request.CodeFrom,
            CurrencyCodeTo = request.CodeTo,
            TransactionType = TransactionType.Convert
        };


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
