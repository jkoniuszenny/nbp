using System.Reflection;
using Application.CQRS;
using Autofac;
using FluentValidation;

namespace Infrastructure.IoC.Modules;

public class ValidationsModule: Autofac.Module
{
    protected override void Load(ContainerBuilder builder)
    {
        var assembly = typeof(IMediatrAutoFacMarker)
          .GetTypeInfo()
          .Assembly;

        _ = builder.RegisterAssemblyTypes(assembly)
            .Where(x => x.IsAssignableTo<IValidator>())
            .AsImplementedInterfaces()
            .InstancePerLifetimeScope();

    }
}
