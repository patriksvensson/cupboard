using System;

namespace Cupboard
{
    public sealed class WingetPackage : Resource, IHasPackageName, IHasPackageState
    {
        public string Package { get; set; }
        public PackageState Ensure { get; set; }
        public bool Force { get; set; }
        public string? PackageVersion { get; set; }
        public string? Override { get; set; }

        public WingetPackage(string name)
            : base(name)
        {
            Package = name ?? throw new ArgumentNullException(nameof(name));
        }
    }
}
