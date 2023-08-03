using System;
using System.Threading.Tasks;

namespace Cupboard;

public interface IResourceProvider
{
    Type ResourceType { get; }

    Resource Create(string name);

    bool RequireAdministrator(FactCollection facts);
    bool CanRun(FactCollection facts);
    Task<ResourceState> RunAsync(IExecutionContext context, Resource resource);
}
