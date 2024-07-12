namespace Cupboard;

[PublicAPI]
public static class IEnumerableExtensions
{
    public static IReadOnlyList<T> ToReadOnlyList<T>(this IEnumerable<T> source)
    {
        return source as IReadOnlyList<T>
            ?? new List<T>(source);
    }
}
