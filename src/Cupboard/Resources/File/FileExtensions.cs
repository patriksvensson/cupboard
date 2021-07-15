using Spectre.IO;

namespace Cupboard
{
    public static class FileExtensions
    {
        public static IResourceBuilder<File> Source(this IResourceBuilder<File> builder, FilePath path)
        {
            return builder.Configure(file => file.Source = path);
        }

        public static IResourceBuilder<File> SymbolicLink(this IResourceBuilder<File> builder)
        {
            return builder.Configure(file => file.SymbolicLink = true);
        }

        public static IResourceBuilder<File> Ensure(this IResourceBuilder<File> builder, FileState state)
        {
            return builder.Configure(file => file.Ensure = state);
        }

        public static IResourceBuilder<File> Permissions(this IResourceBuilder<File> builder, string chmod)
        {
            var permissions = ChmodParser.Parse(chmod);
            return builder.Configure(file => file.Permissions = permissions);
        }

        public static IResourceBuilder<File> Permissions(this IResourceBuilder<File> builder, Chmod permissions)
        {
            return builder.Configure(file => file.Permissions = permissions);
        }
    }
}
