using Application.Interfaces.Repositories;
using Domain.Entities;
using FluentValidation;
using MongoDB.Driver.Linq;
using Shared.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.CQRS.Wallet.Commands.WalletCreate;

public class WalletCreateValidation : AbstractValidator<WalletCreateCommand>
{
    private readonly IAsyncRepository _repository;

    public WalletCreateValidation(CurrencySettings currencySettings, IAsyncRepository repository)
    {
        _repository = repository;

        RuleFor(x => x.Name).NotEmpty().WithMessage("Nazwa portfela jest wymagana");

        RuleFor(x => x.Name).MustAsync(async (m, cancelation) => !(await IsWalletNameExists(m))).WithMessage(n=>$"Portfel o takiej nazwie {n.Name} już istnieje");

        RuleForEach(x => x.InitValue).ChildRules(c =>
        {
            c.RuleFor(r => r.Code)
                .Must(m => currencySettings.Codes.Select(s=>s.Code).Contains(m, StringComparer.InvariantCultureIgnoreCase))
                .WithMessage(t => $"Przekazany kod waluty: {t.Code} nie znajduje się w słowniku");
            c.RuleFor(r => r.Value).GreaterThan(0).WithMessage("Wartości początkowe powinny być większe niż 0");
        });

        RuleFor(x => x.InitValue)
            .Must(values => values.Select(v => v.Code.ToLower()).Distinct().Count() == values.Length)
            .WithMessage("Lista kodów walut zawiera duplikaty.");
    }

    private async Task<bool> IsWalletNameExists(string name) => 
        await (await _repository.AsQueryable<Wallets>())
            .AnyAsync(a => a.Name.ToLower() == name.ToLower());
}
