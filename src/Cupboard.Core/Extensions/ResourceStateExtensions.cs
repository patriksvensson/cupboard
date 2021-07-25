using System;

namespace Cupboard
{
    public static class ResourceStateExtensions
    {
        public static bool IsError(this ResourceState state)
        {
            return state switch
            {
                ResourceState.Unknown => true,
                ResourceState.Changed => false,
                ResourceState.Unchanged => false,
                ResourceState.Error => true,
                ResourceState.Skipped => false,
                ResourceState.Executed => false,
                _ => throw new InvalidOperationException($"Unknown resource state '{state}'"),
            };
        }
    }
}
