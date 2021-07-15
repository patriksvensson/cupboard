using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;

namespace Cupboard.Internal
{
    internal static class ServiceCollectionExtensions
    {
        internal static IServiceCollection RegisterInheritedClasses<TService>(this IServiceCollection services)
            where TService : class
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (!type.IsAbstract && typeof(TService).IsAssignableFrom(type))
                    {
                        services.AddSingleton(typeof(TService), type);
                    }
                }
            }

            return services;
        }

        internal static IServiceCollection RegisterImplementedInterfaces<TService>(this IServiceCollection services)
        {
            if (!typeof(TService).IsInterface)
            {
                throw new InvalidOperationException($"The type '{typeof(TService).FullName}' is not an interface.");
            }

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (!type.IsInterface && !type.IsAbstract && typeof(TService).IsAssignableFrom(type))
                    {
                        services.AddSingleton(typeof(TService), type);
                    }
                }
            }

            return services;
        }

        internal static IServiceCollection RegisterAllOf<TService>(this IServiceCollection services)
        {
            var serviceType = typeof(TService);
            var isInterface = serviceType.IsInterface;

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var type in assemblies.SelectMany(assembly => assembly.GetTypes()))
            {
                if (isInterface && type.IsInterface)
                {
                    continue;
                }

                if (!type.IsAbstract && serviceType.IsAssignableFrom(type))
                {
                    services.AddSingleton(serviceType, type);
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
