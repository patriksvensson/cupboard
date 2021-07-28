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

            context.Resource<RegistryValue>("Disable Game bar tips")
                .Path(@"HKCU:\SOFTWARE\Microsoft\GameBar")
                .Value("ShowStartupPanel")
                .Data(0, RegistryValueKind.DWord);

            context.Resource<RegistryValue>("Disable Bing suggestions")
                .Path(@"HKCU:\Software\Policies\Microsoft\Windows\Explorer")
                .Value("DisableSearchBoxSuggestions")
                .Data(1, RegistryValueKind.DWord);

            context.Resource<RegistryValue>("Disable Bing search")
                .Path(@"HKCU:\SOFTWARE\Microsoft\Windows\CurrentVersion\Search")
                .Value("BingSearchEnabled")
                .Data(0, RegistryValueKind.DWord);

            context.Resource<RegistryValue>("Disable lock screen")
                .Path(@"HKLM:\SOFTWARE\Policies\Microsoft\Windows\Personalization")
                .Value("NoLockScreen")
                .Data(1, RegistryValueKind.DWord);

            context.Resource<RegistryValue>("Disable People in taskbar")
                .Path(@"HKCU:\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Advanced\People")
                .Value("PeopleBand")
                .Ensure(RegistryKeyState.DoNotExist);

            // File Explorer options
            context.Resource<RegistryValue>("Show file extensions in File Explorer.")
                .Path(@"HKCU:\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced")
                .Value("HideFileExt")
                .Data(0, RegistryValueKind.DWord);

            context.Resource<RegistryValue>("Show hidden files in File Explorer")
                .Path(@"HKCU:\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced")
                .Value("Hidden")
                .Data(1, RegistryValueKind.DWord);

            context.Resource<RegistryValue>("Show full path in File Explorer title bar")
                .Path(@"HKCU:\Software\Microsoft\Windows\CurrentVersion\Explorer\CabinetState")
                .Value("FullPathAddress")
                .Data(1, RegistryValueKind.DWord);
        }
    }
}
