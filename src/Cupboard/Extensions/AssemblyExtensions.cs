namespace Cupboard;

internal static class AssemblyExtensions
{
    public static bool TryGetTypes(this Assembly assembly, [NotNullWhen(true)] out Type[]? types)
    {
        try
        {
            types = assembly.GetTypes();
            return true;
        }
        catch
        {
            types = null;
            return false;
        }
    }
}