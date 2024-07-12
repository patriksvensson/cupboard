namespace Cupboard;

internal sealed class ColorPalette<T>
    where T : notnull
{
    private readonly Dictionary<T, Color> _colors;

    public ColorPalette(IEnumerable<T> items, IEqualityComparer<T>? comparer = null)
    {
        _colors = new Dictionary<T, Color>(comparer ?? EqualityComparer<T>.Default);

        var random = new Random(DateTime.Now.Millisecond);
        var colors = new HashSet<Color>();
        foreach (var item in items)
        {
            Color color;
            while (true)
            {
                color = new Color(
                    (byte)random.Next(70, 200),
                    (byte)random.Next(100, 225),
                    (byte)random.Next(100, 230));

                if (colors.Add(color))
                {
                    break;
                }
            }

            _colors[item] = color;
        }
    }

    public Color GetColor(T item)
    {
        if (_colors.TryGetValue(item, out var color))
        {
            return color;
        }

        return Color.White;
    }
}
