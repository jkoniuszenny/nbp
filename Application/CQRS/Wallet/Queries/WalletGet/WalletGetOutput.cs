namespace Application.CQRS.Wallet.Queries.WalletGet;

public record WalletGetOutput
(
    string Name,
    string Id,
    IEnumerable<WalletGetCurrency> Currency
);

public record WalletGetCurrency
(
    string Name,
    string Code,
    double Value
);
