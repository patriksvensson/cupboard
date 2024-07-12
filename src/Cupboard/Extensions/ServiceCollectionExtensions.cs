namespace Cupboard;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddModule<T>(this IServiceCollection services)
        where T : ServiceModule, new()
    {
        if (services is null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        var module = new T();
        module.Configure(services);
        return services;
    }

    internal static IServiceCollection AddAll<TService>(this IServiceCollection services, Assembly? assembly = null)
    {
        var serviceType = typeof(TService);
        var isInterface = serviceType.IsInterface;

        var assemblies = assembly != null
            ? new Assembly[] { assembly }
            : AppDomain.CurrentDomain.GetAssemblies();

        foreach (var type in assemblies.SelectMany(a => a.GetTypes()))
        {
            if (isInterface && type.IsInterface)
            {
                continue;
            }

            if (type.IsAbstract || !serviceType.IsAssignableFrom(type))
            {
                continue;
            }

            if (services.All(x => x.ImplementationType != type))
            {
                services.AddSingleton(serviceType, type);
            }
        }

        return services;
    }

    internal static IServiceCollection AddCommandLine<TCommand>(
        this IServiceCollection services,
        Action<IConfigurator>? configurator = null)
            where TCommand : class, ICommand
    {
        var app = new CommandApp<TCommand>(new TypeRegistrar(services));

        if (configurator != null)
        {
            app.Configure(configurator);
        }

        services.AddSingleton<ICommandApp>(app);

        return services;
    }
}
