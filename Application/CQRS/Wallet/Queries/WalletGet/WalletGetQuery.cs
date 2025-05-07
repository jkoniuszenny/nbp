using Shared.Common;

namespace Application.CQRS.Wallet.Queries.WalletGet;

public class WalletGetQuery : QueryStringParser<WalletGetQuery>, IRequest<GlobalResponse<IEnumerable<WalletGetOutput>>>
{
    public string? WalletName { get; set; }
}
