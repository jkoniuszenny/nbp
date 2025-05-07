using Application.CQRS.Wallet.Queries.WalletGet;
using FastEndpoints.Configuration;
using FastEndpoints.Enum;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EndpointsController.Endpoints.Wallet;

public class WalletGet : FastEndpoint
{
    //created 5/7/2025 11:02:14 AM by Jakub.Koniuszenny
    public WalletGet()
    {
        Method = HttpRequestMethodTypes.Get;
        Url = "/WalletGet";
        Name = "WalletGet";
        Tag = "Wallet";
    }

    /// <summary>
    /// WalletGet
    /// </summary>
    /// <param name="mediator"></param>
    /// <param name="fromQuery"></param>
    /// <param name="query"></param>
    /// <returns></returns>
    //[Authorize]
    public async Task<IResult> ExecuteAsync(IMediator mediator, WalletGetQuery query, [FromQuery] WalletGetQuery? fromQuery)
    {
        var result = await mediator.Send(query);
        fromQuery?.ToString();
        return Results.Ok(result);
    }
}
