using Microsoft.Extensions.DependencyInjection;

namespace Cupboard;

[PublicAPI]
public abstract class ServiceModule
{
    public abstract void Configure(IServiceCollection services);
}
