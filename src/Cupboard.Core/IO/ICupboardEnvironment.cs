namespace Cupboard;

public interface ICupboardEnvironment : IEnvironment
{
    FilePath GetTempFilePath();
}
