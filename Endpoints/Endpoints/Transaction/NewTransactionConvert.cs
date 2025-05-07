using Application.CQRS.Transaction.Commands.NewTransactionConvert;
using FastEndpoints.Configuration;
using FastEndpoints.Enum;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace EndpointsController.Endpoints.Transaction;

public class NewTransactionConvert : FastEndpoint
{
    //created 5/7/2025 12:58:34 PM by Jakub.Koniuszenny
    public NewTransactionConvert()
    {
        Method = HttpRequestMethodTypes.Put;
        Url = "/NewTransactionConvert";
        Name = "NewTransactionConvert";
        Tag = "Transaction";
    }

    /// <summary>
    /// NewTransactionConvert
    /// </summary>
    /// <param name="mediator"></param>
    /// <param name="input"></param>
    /// <returns></returns>
    //[Authorize]
    public async Task<IResult> ExecuteAsync(IMediator mediator, NewTransactionConvertInput input)
    {
        var result = await mediator.Send(new NewTransactionConvertCommand(input));

        return Results.Ok(result);
    }
}
