namespace Application.CQRS.Transaction.Commands.NewTransactionWithdrawPln;

public record NewTransactionWithdrawPlnInput
(
    string WalletId,
    double Value
);
