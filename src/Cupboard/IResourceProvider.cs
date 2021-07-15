using System;

namespace Cupboard
{
    public interface IResourceProvider
    {
        Type ResourceType { get; }

        Resource Create(string name);

        bool CanRun(FactCollection facts);
        ResourceState Run(IExecutionContext context, Resource resource);
    }
}
