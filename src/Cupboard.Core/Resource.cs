using System;

namespace Cupboard
{
    public abstract class Resource : IResourceIdentity
    {
        public Type ResourceType => GetType();

        public string Name { get; }
        public bool RequireAdministrator { get; set; }

        public ErrorOptions Error { get; set; } = ErrorOptions.Abort;
        public RebootOptions Reboot { get; set; } = RebootOptions.Reboot;

        protected Resource(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }
    }
}
