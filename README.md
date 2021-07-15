# Cupboard

A tool to provision local environments to a desired state, using the .NET SDK.

**⚠ Disclaimer:**  
Currently in preview. Use at own risk.

## Introduction

In Cupboard, you define `catalogs`, `manifests`, and `resources`.  

* 🍎 **Resources**
  A resource describes the desired state for some part of a system.  
  This might be a file, directory, package, etc.
* 🌳 **Manifests**  
  A manifest contains one or many resources.
* 🌍 **Catalogs**  
  A catalog contains one or many manifests.

A resource describes something that should be created/updated/deleted
in an environment.

### 1. Creating a manifest

Let's start by creating a manifest that defines what should be run.
Note that resources don't do anything when they're declared. They 
describe what WILL be done, once the catalog(s) are executed.

```csharp
public sealed class Chocolatey : Manifest
{
    public override void Execute(ManifestContext context)
    {
        // Download
        context.Resource<Download>("https://chocolatey.org/install.ps1")
            .ToFile("~/install-chocolatey.ps1");

        // Set execution policy
        context.Resource<RegistryKey>("Set execution policy")
            .Path(@"HKLM:\SOFTWARE\Microsoft\PowerShell\1\ShellIds\Microsoft.PowerShell\ExecutionPolicy")
            .Value("Unrestricted", RegistryKeyValueKind.String);

        // Install
        context.Resource<PowerShellScript>("Install Chocolatey")
            .Script("~/install-chocolatey.ps1")
            .Unless("if (Test-Path \"$($env:ProgramData)/chocolatey/choco.exe\") { exit 1 }")
            .After<RegistryKey>("Set execution policy")
            .After<Download>("https://chocolatey.org/install.ps1");

        // Install VSCode via Chocolatey
        context.Resource<ChocolateyPackage>("vscode")
            .Ensure(PackageState.Installed)
            .After<PowerShellScript>("Install Chocolatey");
    }
```

### 2. Creating a catalog

When we have a `manifest`, we must create a `catalog` containing the manifest.  
We'll also add a condition that excludes non-Windows environments.

```csharp
public sealed class MyWindowsComputer : Catalog
{
    public override void Execute(CatalogContext context)
    {
        if (context.Facts["os.platform"] != OSPlatform.Windows) 
        {
            return;
        }

        context.UseManifest<Chocolatey>();
    }
}
```

### 3. Creating the entry-point

To run the application, we need to add an application entry-point and create a `CupboardHost`.  
We must also explicitly add the catalogs to the `CupboardHostBuilder` instance.

```csharp
public static class Program
{
    public static int Main(string[] args)
    {
        return CupboardHost.CreateBuilder()
            .AddCatalog<MyWindowsComputer>()
            .Build()
            .Run(args);
    }
}
```

### 4. Running the configuration

Now, open up a terminal in administratorr mode and execute the build.

```
> dotnet run -- --verbosity diagnostic
```

Cupboard will show you a summary of what will be changed, which you will
have to approve.

```
❯ dotnet run -- --verbosity diagnostic

┌─Plan───────────────────────────────────────────────────────────────────────────────┐
│                                                                                    │
│   #  │ Provider                 │ Resource                                         │
│  ────┼──────────────────────────┼────────────────────────────────────────────────  │
│   1  │ RegistryKey              │ Set execution policy                             │
│   2  │ Download                 │ https://chocolatey.org/install.ps1               │
│   3  │ PowerShellScript         │ Install Chocolatey                               │
│   4  │ ChocolateyPackage        │ vscode                                           │
│                                                                                    │
└────────────────────────────────────────────────────────────────────────────────────┘
┌─Breakdown──────────────────────────────────────────────────────────────────────────┐
│                                                                                    │
│  ████████████████████████████████████████████████████████████████████████████████  │
│  ■ RegistryKey 1   ■ Download 1   ■ PowerShellScript 1   ■ ChocolateyPackage 1     │
│                                                                                    │
└────────────────────────────────────────────────────────────────────────────────────┘

WARNING: This will change the state of the current machine.
Are you sure you want to continue? [y/n] (n): _
```

## Building

We're using [Cake](https://github.com/cake-build/cake) as a 
[dotnet tool](https://docs.microsoft.com/en-us/dotnet/core/tools/global-tools) 
for building. So make sure that you've restored Cake by running 
the following in the repository root:

```
> dotnet tool restore
```

After that, running the build is as easy as writing:

```
> dotnet cake
```

## Copyright

Copyright (c) 2021 Patrik Svensson