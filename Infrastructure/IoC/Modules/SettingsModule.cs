
using Autofac;
using Microsoft.Extensions.Configuration;
using Shared.Settings;
using Shared.Extensions;

namespace Infrastructure.IoC.Modules;

public class SettingsModule : Autofac.Module
{
    private readonly IConfiguration _configuration;

    public SettingsModule(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    protected override void Load(ContainerBuilder builder)
        => builder.RegisterSettings(_configuration, typeof(ISettings), "Shared");
}

