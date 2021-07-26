using Spectre.IO;

namespace Cupboard
{
    public static class FileSystemExtensions
    {
        public static bool CreateSymbolicLinkSafe(this IFileProvider fileProvider, FilePath source, FilePath destination)
        {
            try
            {
                fileProvider.CreateSymbolicLink(source, destination);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
