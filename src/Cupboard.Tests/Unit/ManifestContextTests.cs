using System;
using Cupboard.Testing;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Cupboard.Tests.Unit;

public sealed class ManifestContextTests
{
    public sealed class Resources
    {
        public sealed class DummyResource : Resource
        {
            public int Foo { get; set; }
            public string Bar { get; set; }

            public DummyResource(string name)
                : base(name)
            {
            }
        }

        public sealed class DummyResourceProvider : ResourceProvider<DummyResource>
        {
            private readonly ICupboardLogger _logger;

            public DummyResourceProvider(ICupboardLogger logger)
            {
                _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            }

            public override DummyResource Create(string name)
            {
                return new DummyResource(name);
            }

            public override ResourceState Run(IExecutionContext context, DummyResource resource)
            {
                _logger.Information($"{resource.Name}: Foo={resource.Foo}, Bar={resource.Bar}");
                return ResourceState.Executed;
            }
        }
    }

    public sealed class Manifests
    {
        public sealed class Default : Manifest
        {
            public override void Execute(ManifestContext context)
            {
                context.Resources(new Resources.DummyResource[]
                {
                    new("baz") { Foo = 1, Bar = "BAR" },
                    new("qux") { Foo = 2, Bar = "QUX" },
                });
            }
        }
    }

    [Fact]
    public void Can_Add_Multiple_Resources_At_Once()
    {
        // Given
        var fixture = new CupboardFixture();
        fixture.Security.IsAdmin = true;
        fixture.Process.RegisterDefaultResult(new ProcessRunnerResult(0));
        fixture.Configure(ctx => ctx.UseManifest<Manifests.Default>());
        fixture.Register(services =>
        {
            services.AddSingleton<IResourceProvider, Resources.DummyResourceProvider>();
        });

        // When
        var result = fixture.Run("-y");

        // Then
        result.ExitCode.ShouldBe(0);
        fixture.Logger.WasLogged("baz: Foo=1, Bar=BAR");
        fixture.Logger.WasLogged("qux: Foo=2, Bar=QUX");
    }
}
