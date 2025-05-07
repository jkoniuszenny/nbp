namespace Application.CQRS.Transaction.Commands.NewTransactionSubtract;

public record NewTransactionSubtractInput
(
    string WalletId,
    string Code,
    decimal Value
);
