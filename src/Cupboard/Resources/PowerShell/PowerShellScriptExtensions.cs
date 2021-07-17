using Spectre.IO;

namespace Cupboard
{
    public static class PowerShellScriptExtensions
    {
        public static IResourceBuilder<PowerShellScript> Script(this IResourceBuilder<PowerShellScript> builder, FilePath file)
        {
            builder.Configure(res => res.ScriptPath = file);
            return builder;
        }

        public static IResourceBuilder<PowerShellScript> Flavor(this IResourceBuilder<PowerShellScript> builder, PowerShellFlavor flavor)
        {
            builder.Configure(res => res.Flavor = flavor);
            return builder;
        }

        public static IResourceBuilder<PowerShellScript> Unless(this IResourceBuilder<PowerShellScript> builder, string script)
        {
            builder.Configure(res => res.Unless = script);
            return builder;
        }
    }
}
