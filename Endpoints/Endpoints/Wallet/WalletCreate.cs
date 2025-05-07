using Application.CQRS.Wallet.Commands.WalletCreate;
using FastEndpoints.Configuration;
using FastEndpoints.Enum;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace EndpointsController.Endpoints.Wallet;

public class WalletCreate : FastEndpoint
{
    //created 5/7/2025 8:59:41 AM by Jakub.Koniuszenny
    public WalletCreate()
    {
        Method = HttpRequestMethodTypes.Post;
        Url = "/WalletCreate";
        Name = "WalletCreate";
        Tag = "Wallet";
    }

    /// <summary>
    /// WalletCreate
    /// </summary>
    /// <param name="mediator"></param>
    /// <param name="input"></param>
    /// <returns></returns>
    //[Authorize]
    public async Task<IResult> ExecuteAsync(IMediator mediator, WalletCreateInput input)
    {
        var result = await mediator.Send(new WalletCreateCommand(input));

        return Results.Ok(result);
    }
}
