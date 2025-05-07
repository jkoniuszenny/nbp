namespace Application.CQRS.Transaction.Commands.NewTransaction;

public record NewTransactionAddInput
(
    string WalletId,
    string Code,
    double Value
);
