using Application.CQRS.Transaction.Commands.NewTransaction;
using FastEndpoints.Configuration;
using FastEndpoints.Enum;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace EndpointsController.Endpoints.Transaction;

public class NewTransactionAdd : FastEndpoint
{
    //created 5/7/2025 11:27:56 AM by Jakub.Koniuszenny
    public NewTransactionAdd()
    {
        Method = HttpRequestMethodTypes.Post;
        Url = "/NewTransactionAdd";
        Name = "NewTransactionAdd";
        Tag = "Transaction";
    }

    /// <summary>
    /// NewTransaction
    /// </summary>
    /// <param name="mediator"></param>
    /// <param name="input"></param>
    /// <returns></returns>
    //[Authorize]
    public async Task<IResult> ExecuteAsync(IMediator mediator, NewTransactionAddInput input)
    {
        var result = await mediator.Send(new NewTransactionAddCommand(input));

        return Results.Ok(result);
    }
}
