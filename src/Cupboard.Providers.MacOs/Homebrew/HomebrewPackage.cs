namespace Cupboard
{
    public sealed class HomebrewPackage : Resource, IHasPackageState, IHasPackageName
    {
        public string Package { get; set; }
        public bool IsCask { get; set; }
        public PackageState Ensure { get; set; }
        public bool PreRelease { get; set; }
        public bool IgnoreChecksum { get; set; }
        public bool AllowDowngrade { get; set; }
        public string? PackageParameters { get; set; }
        public string? PackageVersion { get; set; }

        public string Install() => $"install {(IsCask ? "--cask" : string.Empty)} {Package}";
        public string List() => $"ls {Package}";

        public HomebrewPackage(string name)
            : base(name)
        {
            Package = name ?? throw new ArgumentNullException(nameof(name));
        }
    }
}
