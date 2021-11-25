using Microsoft.Extensions.DependencyInjection;

namespace Cupboard;

public abstract class ServiceModule
{
    public abstract void Configure(IServiceCollection services);
}