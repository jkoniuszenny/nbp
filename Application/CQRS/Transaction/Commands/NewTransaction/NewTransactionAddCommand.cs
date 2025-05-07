namespace Application.CQRS.Transaction.Commands.NewTransaction;

public record NewTransactionAddCommand : NewTransactionAddInput, IRequest<GlobalResponse>
{
    public NewTransactionAddCommand(NewTransactionAddInput input) : base(input) { }
}
