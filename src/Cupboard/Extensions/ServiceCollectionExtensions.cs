using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;

namespace Cupboard.Internal
{
    internal static class ServiceCollectionExtensions
    {
        internal static IServiceCollection AddModule<T>(this IServiceCollection services)
            where T : DependencyModule, new()
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            var module = new T();
            module.Register(services);
            return services;
        }

        internal static IServiceCollection AddAll<TService>(this IServiceCollection services, Assembly? assembly = null)
        {
            var serviceType = typeof(TService);
            var isInterface = serviceType.IsInterface;

            var assemblies = assembly != null
                ? new Assembly[] { assembly }
                : AppDomain.CurrentDomain.GetAssemblies();

            foreach (var type in assemblies.SelectMany(assembly => assembly.GetTypes()))
            {
                if (isInterface && type.IsInterface)
                {
                    continue;
                }

                if (!type.IsAbstract && serviceType.IsAssignableFrom(type))
                {
                    if (!services.Any(x => x.ImplementationType == type))
                    {
                        services.AddSingleton(serviceType, type);
                    }
                }
            }

            return services;
        }

        internal static IServiceCollection AddCommandLine(
            this IServiceCollection services,
            Action<IConfigurator> configurator)
        {
            var app = new CommandApp(new TypeRegistrar(services));
            app.Configure(configurator);
            services.AddSingleton<ICommandApp>(app);

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
}
