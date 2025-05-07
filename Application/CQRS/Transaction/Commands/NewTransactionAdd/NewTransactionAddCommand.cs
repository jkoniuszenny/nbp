namespace Application.CQRS.Transaction.Commands.NewTransactionAdd;

public record NewTransactionAddCommand : NewTransactionAddInput, IRequest<GlobalResponse>
{
    public NewTransactionAddCommand(NewTransactionAddInput input) : base(input) { }
}
