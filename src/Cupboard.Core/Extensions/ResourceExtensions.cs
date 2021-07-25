namespace Cupboard
{
    public static class ResourceExtensions
    {
        public static IResourceBuilder<T> OnError<T>(this IResourceBuilder<T> builder, ErrorHandling error)
            where T : Resource
        {
            builder.Configure(res => res.OnError = error);
            return builder;
        }

        public static IResourceBuilder<T> RequireAdministrator<T>(this IResourceBuilder<T> builder)
            where T : Resource
        {
            builder.Configure(res => res.RequireAdministrator = true);
            return builder;
        }
    }
}
