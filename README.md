# Cupboard

_[![Cupboard NuGet Version](https://img.shields.io/nuget/v/cupboard.svg?style=flat&label=NuGet%3A%20Cupboard)](https://www.nuget.org/packages/cupboard)_

A framework for provisioning local environments to a desired state, using the .NET SDK.

## Disclaimer
Currently in preview. **Use at own risk.**

## Introduction

In Cupboard, you define `catalogs`, `manifests`, and `resources`.

* **Resources:**  
  A resource describes the desired state for some part of a system.  
  This might be a file, directory, package, etc.
* **Manifests:**  
  A manifest contains one or many resources.  
  Used to specify how to assemble parts that uses many resources.
* **Catalogs:**  
  A catalog contains one or many manifests.  
  Used as a logical grouping of machines and/or platforms.

### 1. Create a console application

Start by creating a new console application targeting `.NET5.0`.  
Add a reference to the `Cupboard` NuGet package.

```
> dotnet new console
> dotnet add package Cupboard
```

### 2. Create a manifest

Let's start by creating a manifest that defines what should be run.  

Note that resources don't do anything when they're declared.  
They describe what **WILL** be done when executed.

```csharp
public sealed class Chocolatey : Manifest
{
    public override void Execute(ManifestContext context)
    {
        // Download
        context.Resource<Download>("https://chocolatey.org/install.ps1")
            .ToFile("~/install-chocolatey.ps1");

        // Set execution policy
        context.Resource<RegistryValue>("Set execution policy")
            .Path(@"HKLM:\SOFTWARE\Microsoft\PowerShell\1\ShellIds\Microsoft.PowerShell")
            .Value("ExecutionPolicy")
            .Data("Unrestricted", RegistryValueKind.String);

        // Install
        context.Resource<PowerShell>("Install Chocolatey")
            .Script("~/install-chocolatey.ps1")
            .Flavor(PowerShellFlavor.PowerShell)
            .RequireAdministrator()
            .Unless("if (Test-Path \"$($env:ProgramData)/chocolatey/choco.exe\") { exit 1 }")
            .After<RegistryValue>("Set execution policy")
            .After<Download>("https://chocolatey.org/install.ps1");

        // Install VSCode via Chocolatey
        context.Resource<ChocolateyPackage>("vscode")
            .Ensure(PackageState.Installed)
            .After<PowerShell>("Install Chocolatey");
    }
}
```

### 3. Create a catalog

When we have a `Manifest`, we need to create a `Catalog` containing the manifest.  
We'll also add a condition that excludes non-Windows environments.

```csharp
public sealed class WindowsComputer : Catalog
{
    public override void Execute(CatalogContext context)
    {
        if (!context.Facts.IsWindows())
        {
            return;
        }

        context.UseManifest<Chocolatey>();
    }
}
```

### 4. Create the entry-point

To run the application, we need to add an application entry-point and create a `CupboardHost`.  
We must also explicitly add the catalogs to the `CupboardHostBuilder` instance.

```csharp
public static class Program
{
    public static int Main(string[] args)
    {
        return CupboardHost.CreateBuilder()
            .AddCatalog<MyWindowsComputer>()
            .Run(args);
    }
}
```

### 5. Run the configuration

Now, open up a terminal in administrator mode and execute the build.

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
