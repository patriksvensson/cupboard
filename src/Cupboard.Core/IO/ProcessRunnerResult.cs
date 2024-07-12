namespace Cupboard;

[PublicAPI]
public sealed class ProcessRunnerResult
{
    public int ExitCode { get; }
    public string StandardOut { get; }
    public string StandardError { get; }

    public ProcessRunnerResult(int exitCode, string? standardOut = null, string? standardError = null)
    {
        ExitCode = exitCode;
        StandardOut = standardOut ?? string.Empty;
        StandardError = standardError ?? string.Empty;
    }
}
