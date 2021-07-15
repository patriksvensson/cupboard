using Cupboard;

namespace Sandbox
{
    public sealed class WindowsSettings : Manifest
    {
        public override void Execute(ManifestContext context)
        {
            if (!context.Facts["windows"]["sandbox"])
            {
                context.Resource<WindowsFeature>("Windows Sandbox")
                    .FeatureName("Containers-DisposableClientVM")
                    .Ensure(WindowsFeatureState.Enabled);
            }

            context.Resource<RegistryKey>("Disable Game bar tips")
                .Path(@"HKCU:\SOFTWARE\Microsoft\GameBar\ShowStartupPanel")
                .Value(0, RegistryKeyValueKind.DWord);

            context.Resource<RegistryKey>("Disable Bing suggestions")
                .Path(@"HKCU:\Software\Policies\Microsoft\Windows\Explorer\DisableSearchBoxSuggestions")
                .Value(1, RegistryKeyValueKind.DWord);

            context.Resource<RegistryKey>("Disable Bing search")
                .Path(@"HKCU:\SOFTWARE\Microsoft\Windows\CurrentVersion\Search\BingSearchEnabled")
                .Value(0, RegistryKeyValueKind.DWord);

            context.Resource<RegistryKey>("Disable lock screen")
                .Path(@"HKLM:\SOFTWARE\Policies\Microsoft\Windows\Personalization\NoLockScreen")
                .Value(1, RegistryKeyValueKind.DWord);

            context.Resource<RegistryKey>("Disable People in taskbar")
                .Path(@"HKCU:\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Advanced\People\PeopleBand")
                .Ensure(RegistryKeyState.DoNotExist);
        }
    }
}
