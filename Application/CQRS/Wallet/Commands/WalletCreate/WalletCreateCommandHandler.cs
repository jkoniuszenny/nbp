using Application.Interfaces.Providers;
using Application.Interfaces.Repositories;
using Domain.Entities;
using MediatR;
using Shared.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.CQRS.Wallet.Commands.WalletCreate;

internal sealed class WalletCreateCommandHandler : IRequestHandler<WalletCreateCommand, GlobalResponse>
{
    private readonly IAsyncRepository _repository;
    private readonly IValidator<WalletCreateCommand> _validator;
    private readonly CurrencySettings _currencySettings;

    public WalletCreateCommandHandler(
        IAsyncRepository repository,
         IValidator<WalletCreateCommand> validator,
         CurrencySettings currencySettings)
    {
        _repository = repository;
        _validator = validator;
        _currencySettings = currencySettings;
    }

    public async Task<GlobalResponse> Handle(WalletCreateCommand request, CancellationToken cancellationToken)
    {

        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            return await GlobalResponse.ValidationsAsync(validationResult.Errors.Select(x => x.ErrorMessage));

        var newWallet = new Wallets
        {
            Name = request.Name,
            Currencies = [.. request.InitValue.Select(s => new Currency
            {
                Name = _currencySettings.Codes.First(f=>f.Code == s.Code).Name,
                Code = s.Code,
                Value = s.Value
            })]
        };

        await _repository.InsertList([newWallet]);

        return await GlobalResponse.SuccessAsync();
    }
}
