namespace Cupboard;

[PublicAPI]
public interface ICupboardEnvironment : IEnvironment
{
    FilePath GetTempFilePath();
}
