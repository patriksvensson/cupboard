namespace Cupboard;

[PublicAPI]
public sealed class Fact
{
    private static Fact DefaultFact { get; } = new Fact(null, string.Empty);

    public string Name { get; }
    public string FullName { get; }
    public object? Value { get; internal set; }
    public Fact this[string key] => GetChild(key);

    internal Fact? Parent { get; }
    internal Dictionary<string, Fact> Children { get; }

    internal Fact(Fact? parent, string id, object? value = null)
    {
        Parent = parent;
        Name = id ?? throw new ArgumentNullException(nameof(id));
        Value = value;
        Children = new Dictionary<string, Fact>(StringComparer.OrdinalIgnoreCase);
        FullName = GetFullName();
    }

    public T? As<T>(T? defaultValue = default)
        where T : notnull
    {
        if (Value is T result)
        {
            return result;
        }

        return defaultValue;
    }

    private string GetFullName()
    {
        var stack = new Stack<string>();
        var current = this;
        while (current != null)
        {
            if (!string.IsNullOrWhiteSpace(current.Name))
            {
                stack.Push(current.Name);
            }

            current = current.Parent;
        }

        return string.Join(".", stack);
    }

    private Fact GetChild(string key)
    {
        var parts = key.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length > 1)
        {
            // Get full path
            var queue = new Queue<string>(key.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries));
            var root = this;
            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                if (root.Children.TryGetValue(current, out var node))
                {
                    root = node;
                }
                else
                {
                    return DefaultFact;
                }
            }

            return root;
        }
        else
        {
            Children.TryGetValue(key, out var fact);
            return fact ?? DefaultFact;
        }
    }

    public static implicit operator string(Fact fact)
    {
        return fact.Value as string ?? string.Empty;
    }

    public static implicit operator int(Fact fact)
    {
        return fact.Value is int value ? value : 0;
    }

    public static implicit operator decimal(Fact fact)
    {
        return fact.Value is decimal value ? value : 0M;
    }

    public static implicit operator double(Fact fact)
    {
        return fact.Value is double value ? value : 0D;
    }

    public static implicit operator float(Fact fact)
    {
        return fact.Value is float value ? value : 0F;
    }

    public static implicit operator bool(Fact fact)
    {
        return fact.Value is bool value && value;
    }
}
