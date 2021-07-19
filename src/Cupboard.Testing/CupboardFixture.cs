using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Spectre.Console.Testing;
using Spectre.IO;

namespace Cupboard.Testing
{
    public sealed class CupboardFixture
    {
        private readonly List<LambdaCatalog> _catalogs;
        private readonly FakeReportSubscriber _interceptor;
        private readonly FakeFactBuilder _factBuilder;

        public TestConsole Console { get; }
        public FakeLogger Logger { get; }
        public FakeProcessRunner Process { get; }
        public FakeSecurityPrincipal Security { get; }
        public FakeEnvironmentRefresher EnvironmentRefresher { get; }
        public FakeCupboardFileSystem FileSystem { get; }
        public FakeCupboardEnvironment Environment { get; }
        public FakeWindowsRegistry WindowsRegistry { get; }

        public FactCollection Facts => _factBuilder.Facts;

        public CupboardFixture(PlatformFamily family = PlatformFamily.Windows)
        {
            _catalogs = new List<LambdaCatalog>();
            _interceptor = new FakeReportSubscriber();
            _factBuilder = new FakeFactBuilder();

            Console = new TestConsole();
            Logger = new FakeLogger();
            Process = new FakeProcessRunner();
            Security = new FakeSecurityPrincipal();
            EnvironmentRefresher = new FakeEnvironmentRefresher();
            Environment = new FakeCupboardEnvironment(family);
            FileSystem = new FakeCupboardFileSystem(Environment);
            WindowsRegistry = new FakeWindowsRegistry();

            switch (family)
            {
                case PlatformFamily.Windows:
                    Facts.Add("os.platform", OSPlatform.Windows);
                    break;
                case PlatformFamily.Linux:
                    Facts.Add("os.platform", OSPlatform.Linux);
                    break;
                case PlatformFamily.MacOs:
                    Facts.Add("os.platform", OSPlatform.OSX);
                    break;
                case PlatformFamily.Unknown:
                    throw new InvalidOperationException("Unknown platform");
            }
        }

        public void Configure(Action<CatalogContext> action)
        {
            _catalogs.Add(new LambdaCatalog(action));
        }

        public (int ExitCode, Report? Report) Run(params string[] args)
        {
            var result = BuildHost().Run(args);
            return (result, _interceptor.Report);
        }

        private CupboardHost BuildHost()
        {
            var builder = new CupboardHostBuilder(Console);
            builder.PropagateExceptions();
            builder.ConfigureServices(services =>
            {
                // Register fakes
                services.AddSingleton(FileSystem);
                services.AddSingleton(Environment);
                services.AddSingleton(Process);
                services.AddSingleton(EnvironmentRefresher);
                services.AddSingleton(Logger);

                // Register the report interceptor
                services.AddSingleton<IReportSubscriber>(_interceptor);

                // Replace existing services
                services.Replace(ServiceDescriptor.Singleton<IProcessRunner>(_ => Process));
                services.Replace(ServiceDescriptor.Singleton<ISecurityPrincipal>(_ => Security));
                services.Replace(ServiceDescriptor.Singleton<ICupboardFileSystem>(_ => FileSystem));
                services.Replace(ServiceDescriptor.Singleton<ICupboardEnvironment>(_ => Environment));
                services.Replace(ServiceDescriptor.Singleton<IEnvironmentRefresher>(_ => EnvironmentRefresher));
                services.Replace(ServiceDescriptor.Singleton<IWindowsRegistry>(_ => WindowsRegistry));
                services.Replace(ServiceDescriptor.Singleton<ICupboardLogger>(_ => Logger));
                services.Replace(ServiceDescriptor.Singleton<IFactBuilder>(_ => _factBuilder));

                foreach (var catalog in _catalogs)
                {
                    services.AddSingleton<Catalog>(catalog);
                }
            });

            return builder.Build();
        }
    }
}
