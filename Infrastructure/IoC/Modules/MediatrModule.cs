using Application.CQRS;
using Autofac;
using MediatR;
using MediatR.Pipeline;
using System.Reflection;

namespace Infrastructure.IoC.Modules;

public class MediatrModule : Autofac.Module
{
    protected override void Load(ContainerBuilder builder)
    {
        var assembly = typeof(IMediatrAutoFacMarker)
            .GetTypeInfo()
            .Assembly;

        builder.RegisterAssemblyTypes(typeof(IMediator).GetTypeInfo().Assembly).AsImplementedInterfaces();
        var mediatrOpenTypes = new[]
        {
                typeof(IRequestHandler<,>),
                typeof(INotificationHandler<>),
            };

        foreach (var mediatrOpenType in mediatrOpenTypes)
        {
            builder
                .RegisterAssemblyTypes(assembly)
                .AsClosedTypesOf(mediatrOpenType)
                .AsImplementedInterfaces();
        }


        builder.RegisterGeneric(typeof(RequestPostProcessorBehavior<,>)).As(typeof(IPipelineBehavior<,>));
        builder.RegisterGeneric(typeof(RequestPreProcessorBehavior<,>)).As(typeof(IPipelineBehavior<,>));

    }
}

