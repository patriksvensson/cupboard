namespace Cupboard
{
    public interface ICupboardLogger
    {
        Verbosity Verbosity { get; }

        void SetVerbosity(Verbosity verbosity);
        void Log(Verbosity verbosity, LogLevel level, string text);
        void Log(Verbosity verbosity, LogLevel level, string title, string text);
    }
}
