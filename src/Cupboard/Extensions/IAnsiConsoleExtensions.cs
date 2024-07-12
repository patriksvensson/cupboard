namespace Cupboard;

internal static class IAnsiConsoleExtensions
{
    public static bool Confirm(this IAnsiConsole console, string markup, bool defaultValue = true)
    {
        return new ConfirmationPrompt(markup)
        {
            DefaultValue = defaultValue,
        }
        .Show(console);
    }
}
