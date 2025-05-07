using Application.CQRS.Transaction.Commands.NewTransactionWithdrawPln;
using FastEndpoints.Configuration;
using FastEndpoints.Enum;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace EndpointsController.Endpoints.Transaction;

public class NewTransactionWithdrawPln : FastEndpoint
{
    //created 5/7/2025 12:58:34 PM by Jakub.Koniuszenny
    public NewTransactionWithdrawPln()
    {
        Method = HttpRequestMethodTypes.Put;
        Url = "/NewTransactionWithdrawPln";
        Name = "NewTransactionWithdrawPln";
        Tag = "Transaction";
    }

    /// <summary>
    /// NewTransactionWithdrawPln
    /// </summary>
    /// <param name="mediator"></param>
    /// <param name="input"></param>
    /// <returns></returns>
    [Authorize]
    public async Task<IResult> ExecuteAsync(IMediator mediator, NewTransactionWithdrawPlnInput input)
    {
        var result = await mediator.Send(new NewTransactionWithdrawPlnCommand(input));

        return Results.Ok(result);
    }
}
