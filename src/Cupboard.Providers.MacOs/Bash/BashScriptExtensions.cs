using Spectre.IO;

namespace Cupboard
{
    public static class BashScriptExtensions
    {
        public static IResourceBuilder<BashScript> Script(this IResourceBuilder<BashScript> builder, FilePath file)
        {
            builder.Configure(res => res.Script = file);
            return builder;
        }

        public static IResourceBuilder<BashScript> Command(this IResourceBuilder<BashScript> builder, string command)
        {
            builder.Configure(res => res.Command = command);
            return builder;
        }

        public static IResourceBuilder<BashScript> Unless(this IResourceBuilder<BashScript> builder, string script)
        {
            builder.Configure(res => res.Unless = script);
            return builder;
        }
    }
}
