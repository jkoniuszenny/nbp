using Application.CQRS.Transaction.Commands.NewTransactionSubtract;
using FastEndpoints.Configuration;
using FastEndpoints.Enum;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace EndpointsController.Endpoints.Transaction;

public class NewTransactionSubtract : FastEndpoint
{
    //created 5/7/2025 12:33:46 PM by Jakub.Koniuszenny
    public NewTransactionSubtract()
    {
        Method = HttpRequestMethodTypes.Put;
        Url = "/NewTransactionSubtract";
        Name = "NewTransactionSubtract";
        Tag = "Transaction";
    }

    /// <summary>
    /// NewTransactionSubtract
    /// </summary>
    /// <param name="mediator"></param>
    /// <param name="input"></param>
    /// <returns></returns>
    //[Authorize]
    public async Task<IResult> ExecuteAsync(IMediator mediator, NewTransactionSubtractInput input)
    {
        var result = await mediator.Send(new NewTransactionSubtractCommand(input));

        return Results.Ok(result);
    }
}
