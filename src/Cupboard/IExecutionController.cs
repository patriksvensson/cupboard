namespace Cupboard;

public interface IExecutionController
{
    ExecutionAction GetAction(IResourceProvider provider, Resource resource);
}

internal sealed class DefaultExecutionController : IExecutionController
{
    public ExecutionAction GetAction(IResourceProvider provider, Resource resource)
    {
        return ExecutionAction.Run;
    }
}

internal sealed class InteractiveExecutionController : IExecutionController
{
    private readonly IAnsiConsole _console;

    public InteractiveExecutionController(IAnsiConsole console)
    {
        _console = console ?? throw new ArgumentNullException(nameof(console));
    }

    public ExecutionAction GetAction(IResourceProvider provider, Resource resource)
    {
        var name = $"[green]{provider.ResourceType.Name.EscapeMarkup()}[/]::[blue]{resource.Name.EscapeMarkup()}[/]";
        var prompt = new TextPrompt<string>($"Do you want to run {name}?")
            .InvalidChoiceMessage("[red]Please select one of the available options[/]")
            .ValidationErrorMessage("[red]Please select one of the available options[/]")
            .ShowChoices(true)
            .ShowDefaultValue()
            .DefaultValue("n")
            .AddChoice("y")
            .AddChoice("n")
            .AddChoice("a");

        var result = prompt.Show(_console);
        if (result.Equals("y"))
        {
            return ExecutionAction.Run;
        }
        else if (result.Equals("a"))
        {
            return ExecutionAction.Abort;
        }
        else
        {
            return ExecutionAction.Skip;
        }
    }
}
