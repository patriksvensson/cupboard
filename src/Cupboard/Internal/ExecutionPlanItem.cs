using System;

namespace Cupboard.Internal
{
    internal sealed class ExecutionPlanItem
    {
        public IResourceProvider Provider { get; }
        public Resource Resource { get; }

        public ExecutionPlanItem(IResourceProvider provider, Resource resource)
        {
            Provider = provider ?? throw new ArgumentNullException(nameof(provider));
            Resource = resource ?? throw new ArgumentNullException(nameof(resource));
        }
    }
}
