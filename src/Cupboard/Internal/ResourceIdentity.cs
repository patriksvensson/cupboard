using System;

namespace Cupboard.Internal
{
    internal sealed class ResourceIdentity : IResourceIdentity
    {
        public Type ResourceType { get; }
        public string Name { get; }

        public ResourceIdentity(Type resourceType, string name)
        {
            ResourceType = resourceType;
            Name = name;
        }

        public ResourceIdentity(Resource resource)
        {
            ResourceType = resource.ResourceType;
            Name = resource.Name;
        }

        public ResourceIdentity(Tuple<Type, string> tuple)
        {
            ResourceType = tuple.Item1;
            Name = tuple.Item2;
        }
    }
}
