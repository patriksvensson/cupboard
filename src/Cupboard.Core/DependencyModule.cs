using Microsoft.Extensions.DependencyInjection;

namespace Cupboard
{
    public abstract class DependencyModule
    {
        public abstract void Register(IServiceCollection services);
    }
}
