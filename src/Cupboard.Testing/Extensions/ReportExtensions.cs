namespace Cupboard.Testing;

public static class ReportExtensions
{
    public static ResourceState GetState<TResource>(this Report report, string name)
        where TResource : Resource
    {
        if (report == null)
        {
            return ResourceState.Unknown;
        }

        var item = report.Items.SingleOrDefault(
            x => x.Resource.GetType() == typeof(TResource)
                && x.Resource.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

        return item?.State ?? ResourceState.Unknown;
    }
}
