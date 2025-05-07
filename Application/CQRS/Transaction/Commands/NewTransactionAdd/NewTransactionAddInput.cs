namespace Application.CQRS.Transaction.Commands.NewTransactionAdd;

public record NewTransactionAddInput
(
    string WalletId,
    string Code,
    decimal Value
);
