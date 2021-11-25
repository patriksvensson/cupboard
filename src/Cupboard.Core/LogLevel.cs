namespace Cupboard;

public enum LogLevel
{
    /// <summary>
    /// Severe errors that cause premature termination.
    /// </summary>
    Fatal,

    /// <summary>
    /// Other runtime errors or unexpected conditions.
    /// </summary>
    Error,

    /// <summary>
    /// Use of deprecated APIs, poor use of API, 'almost' errors, other runtime
    /// situations that are undesirable or unexpected, but not necessarily "wrong".
    /// </summary>
    Warning,

    /// <summary>
    /// Interesting runtime events.
    /// </summary>
    Information,

    /// <summary>
    /// Detailed information on the flow through the system.
    /// </summary>
    Verbose,

    /// <summary>
    /// Most detailed information.
    /// </summary>
    Debug,
}