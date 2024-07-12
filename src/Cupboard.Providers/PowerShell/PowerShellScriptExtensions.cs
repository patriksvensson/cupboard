namespace Cupboard;

[PublicAPI]
public static class PowerShellScriptExtensions
{
    public static IResourceBuilder<PowerShell> Script(this IResourceBuilder<PowerShell> builder, FilePath file)
    {
        builder.Configure(res => res.Script = file);
        return builder;
    }

    public static IResourceBuilder<PowerShell> Command(this IResourceBuilder<PowerShell> builder, string command)
    {
        builder.Configure(res => res.Command = command);
        return builder;
    }

    public static IResourceBuilder<PowerShell> Flavor(this IResourceBuilder<PowerShell> builder, PowerShellFlavor flavor)
    {
        builder.Configure(res => res.Flavor = flavor);
        return builder;
    }

    public static IResourceBuilder<PowerShell> Unless(this IResourceBuilder<PowerShell> builder, string script)
    {
        builder.Configure(res => res.Unless = script);
        return builder;
    }
}
