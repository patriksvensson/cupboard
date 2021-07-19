using Spectre.IO;

namespace Cupboard
{
    public interface ICupboardEnvironment : IEnvironment
    {
        FilePath GetTempFilePath();
    }
}
