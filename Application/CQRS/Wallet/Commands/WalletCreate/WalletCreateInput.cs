namespace Application.CQRS.Wallet.Commands.WalletCreate;

public record WalletCreateInput
(
    string Name,
    WalletCurrency[] InitValue
);

public record WalletCurrency
(
    string Code,
    decimal Value
);
