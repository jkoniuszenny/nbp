namespace Application.CQRS.Transaction.Commands.NewTransactionConvert;

public record NewTransactionConvertCommand : NewTransactionConvertInput, IRequest<GlobalResponse>
{
    public NewTransactionConvertCommand(NewTransactionConvertInput input) : base(input) { }
}
