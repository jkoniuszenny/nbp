namespace Application.CQRS.Transaction.Commands.NewTransactionSubtract;

public record NewTransactionSubtractCommand : NewTransactionSubtractInput, IRequest<GlobalResponse>
{
    public NewTransactionSubtractCommand(NewTransactionSubtractInput input) : base(input) { }
}
