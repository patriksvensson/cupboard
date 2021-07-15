using System;
using System.Threading.Tasks;

namespace Cupboard
{
    public abstract class ResourceProvider<TResource> : IResourceProvider
        where TResource : Resource
    {
        public Type ResourceType => typeof(TResource);

        public virtual bool CanRun(FactCollection facts)
        {
            return true;
        }

        public abstract TResource Create(string name);

        public abstract ResourceState Run(IExecutionContext context, TResource resource);

        ResourceState IResourceProvider.Run(IExecutionContext context, Resource resource)
        {
            return Run(context, (TResource)resource);
        }

        Resource IResourceProvider.Create(string name)
        {
            return Create(name);
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

        public abstract Task<ResourceState> Run(IExecutionContext context, TResource resource);

        ResourceState IResourceProvider.Run(IExecutionContext context, Resource resource)
        {
            return Run(context, (TResource)resource).GetAwaiter().GetResult();
        }

        Resource IResourceProvider.Create(string name)
        {
            return Create(name);
        }
    }
}
