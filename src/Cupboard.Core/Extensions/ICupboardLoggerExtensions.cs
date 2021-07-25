namespace Cupboard
{
    public static class ICupboardLoggerExtensions
    {
        public static void Fatal(this ICupboardLogger logger, string markup)
        {
            logger?.Log(Verbosity.Quiet, LogLevel.Fatal, markup);
        }

        public static void Error(this ICupboardLogger logger, string markup)
        {
            logger?.Log(Verbosity.Quiet, LogLevel.Error, markup);
        }

        public static void Warning(this ICupboardLogger logger, string markup)
        {
            logger?.Log(Verbosity.Minimal, LogLevel.Warning, markup);
        }

        public static void Information(this ICupboardLogger logger, string markup)
        {
            logger?.Log(Verbosity.Normal, LogLevel.Information, markup);
        }

        public static void Verbose(this ICupboardLogger logger, string markup)
        {
            logger?.Log(Verbosity.Verbose, LogLevel.Verbose, markup);
        }

        public static void Debug(this ICupboardLogger logger, string markup)
        {
            logger?.Log(Verbosity.Diagnostic, LogLevel.Debug, markup);
        }
    }
}
