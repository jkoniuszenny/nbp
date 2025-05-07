using Autofac;
using Infrastructure.Database;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.IoC.Modules;

public class DbContextModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<DatabaseMongoContext>()
            .SingleInstance();

        builder.RegisterType<HttpContextAccessor>()
           .As<IHttpContextAccessor>()
           .SingleInstance();
    }
}

