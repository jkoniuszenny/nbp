namespace Application.CQRS.Transaction.Commands.NewTransactionConvert;

public record NewTransactionConvertInput
(
    string WalletId,
    string CodeFrom,
    string CodeTo,
    decimal Value
);
