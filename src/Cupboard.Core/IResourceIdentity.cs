using System;

namespace Cupboard;

public interface IResourceIdentity
{
    Type ResourceType { get; }

    string Name { get; }
}