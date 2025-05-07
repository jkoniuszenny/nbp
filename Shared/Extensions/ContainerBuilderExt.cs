using Autofac;
using Microsoft.Extensions.Configuration;

namespace Shared.Extensions;

public static class ContainerBuilderExt
{
    public static void RegisterSettings(
        this ContainerBuilder builder,
        IConfiguration configuration,
        Type objectType,
        string projectName = "Shared",
        string classExtendedName = "Settings")
    {
        foreach (Type type in AppDomain.CurrentDomain.Load(projectName).GetTypes()
       .Where(mytype => !mytype.IsAbstract && mytype.GetInterfaces().Contains(objectType)))
        {
            var instance = Activator.CreateInstance(type);

            if (instance is null)
                continue;

            configuration.GetSection(type.Name.Replace(classExtendedName, string.Empty)).Bind(instance);

            builder.RegisterInstance(instance)
            .As(type)
            .SingleInstance();
        }
    }


}
