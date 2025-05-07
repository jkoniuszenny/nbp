namespace Application.CQRS.Transaction.Commands.NewTransactionWithdrawPln;

public record NewTransactionWithdrawPlnCommand : NewTransactionWithdrawPlnInput, IRequest<GlobalResponse>
{
    public NewTransactionWithdrawPlnCommand(NewTransactionWithdrawPlnInput input) : base(input) { }
}
