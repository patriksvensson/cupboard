using System.Collections;
using System.Collections.Generic;

namespace Cupboard;

public sealed class FactCollection : IEnumerable<Fact>
{
    private readonly Fact _root;

    public Fact this[string key] => _root[key];

    public FactCollection()
    {
        _root = new Fact(null, string.Empty, null);
    }

    public void Add(string key, object value)
    {
        var root = _root;

        var queue = new Queue<string>(key.Split('.'));
        while (queue.Count > 0)
        {
            var current = queue.Dequeue();

            if (!root.Children.TryGetValue(current, out var node))
            {
                node = queue.Count == 0
                    ? new Fact(root, current, value)
                    : new Fact(root, current);

                root.Children.Add(current, node);
            }
            else
            {
                if (queue.Count == 0)
                {
                    // Overwrite the value
                    node.Value = value;
                }
            }

            root = node;
        }
    }

    public IEnumerator<Fact> GetEnumerator()
    {
        var stack = new Stack<Fact>();
        stack.Push(_root);
        while (stack.Count > 0)
        {
            var current = stack.Pop();
            if (current.Value != null)
            {
                yield return current;
            }

            foreach (var (_, child) in current.Children)
            {
                stack.Push(child);
            }
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}