using System;

namespace Cupboard;

public interface IResourceBuilder<out TResource>
    where TResource : Resource
{
    IResourceBuilder<TResource> Before<TOther>(string name);
    IResourceBuilder<TResource> After<TOther>(string name);
    IResourceBuilder<TResource> Configure(Action<TResource> action);
}