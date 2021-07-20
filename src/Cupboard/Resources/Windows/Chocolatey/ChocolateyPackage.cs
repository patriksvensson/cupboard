using System;

namespace Cupboard
{
    public sealed class ChocolateyPackage : Resource
    {
        public string Package { get; set; }
        public PackageState Ensure { get; set; }
        public bool PreRelease { get; set; }
        public bool IgnoreChecksum { get; set; }
        public string? PackageParameters { get; set; }

        public ChocolateyPackage(string name)
            : base(name)
        {
            Package = name ?? throw new ArgumentNullException(nameof(name));
        }
    }
}
