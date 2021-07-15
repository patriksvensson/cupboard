using System;
using System.Threading.Tasks;

namespace Cupboard
{
    public abstract class ResourceProvider<TResource> : AsyncResourceProvider<TResource>
        where TResource : Resource
    {
        public abstract ResourceState Run(IExecutionContext context, TResource resource);

        public sealed override Task<ResourceState> RunAsync(IExecutionContext context, TResource resource)
        {
            return Task.FromResult(Run(context, resource));
        }
    }

    public abstract class AsyncResourceProvider<TResource> : IResourceProvider
        where TResource : Resource
    {
        public Type ResourceType => typeof(TResource);

        public virtual bool CanRun(FactCollection facts)
        {
            return true;
        }

        public abstract TResource Create(string name);

        public abstract Task<ResourceState> RunAsync(IExecutionContext context, TResource resource);

        Task<ResourceState> IResourceProvider.RunAsync(IExecutionContext context, Resource resource)
        {
            return RunAsync(context, (TResource)resource);
        }

        Resource IResourceProvider.Create(string name)
        {
            return Create(name);
        }
    }
}
