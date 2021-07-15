using System;

namespace Cupboard
{
    public abstract class Resource : IResourceIdentity
    {
        public Type ResourceType => GetType();

        public string Name { get; }
        public ErrorHandling OnError { get; set; } = ErrorHandling.Abort;

        protected Resource(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }
    }
}
