using Spectre.IO;

namespace Cupboard;

public static class ExecExtensions
{
    public static IResourceBuilder<Exec> Path(this IResourceBuilder<Exec> builder, FilePath file)
    {
        builder.Configure(res => res.Path = file);
        return builder;
    }

    public static IResourceBuilder<Exec> Arguments(this IResourceBuilder<Exec> builder, string args)
    {
        builder.Configure(res => res.Args = args);
        return builder;
    }

    public static IResourceBuilder<Exec> ValidExitCodes(this IResourceBuilder<Exec> builder, params int[] exitCodes)
    {
        builder.Configure(res => res.ValidExitCodes = exitCodes);
        return builder;
    }
}
