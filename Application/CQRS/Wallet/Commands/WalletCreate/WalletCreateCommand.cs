namespace Application.CQRS.Wallet.Commands.WalletCreate;

public record WalletCreateCommand : WalletCreateInput, IRequest<GlobalResponse>
{
    public WalletCreateCommand(WalletCreateInput input) : base(input) { }
}
