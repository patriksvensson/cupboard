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

        public static void Fatal(this ICupboardLogger logger, string title, string markup)
        {
            logger?.Log(Verbosity.Quiet, LogLevel.Fatal, title, markup);
        }

        public static void Error(this ICupboardLogger logger, string title, string markup)
        {
            logger?.Log(Verbosity.Quiet, LogLevel.Error, title, markup);
        }

        public static void Warning(this ICupboardLogger logger, string title, string markup)
        {
            logger?.Log(Verbosity.Minimal, LogLevel.Warning, title, markup);
        }

        public static void Information(this ICupboardLogger logger, string title, string markup)
        {
            logger?.Log(Verbosity.Normal, LogLevel.Information, title, markup);
        }

        public static void Verbose(this ICupboardLogger logger, string title, string markup)
        {
            logger?.Log(Verbosity.Verbose, LogLevel.Verbose, title, markup);
        }

        public static void Debug(this ICupboardLogger logger, string title, string markup)
        {
            logger?.Log(Verbosity.Diagnostic, LogLevel.Debug, title, markup);
        }
    }
}
