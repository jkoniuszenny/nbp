using Autofac;
using Infrastructure.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.IoC.Modules;

public class DbContextModule : Module
{


    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<HttpContextAccessor>()
           .As<IHttpContextAccessor>()
           .SingleInstance();

        builder.RegisterType<DatabaseMongoContext>()
           .InstancePerLifetimeScope();
    }
}

