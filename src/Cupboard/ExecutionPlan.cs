using System.Collections;
using System.Collections.Generic;

namespace Cupboard.Internal
{
    internal sealed class ExecutionPlan : IEnumerable<ExecutionPlanItem>
    {
        private readonly IReadOnlyList<ExecutionPlanItem> _resources;

        public bool RequiresAdministrator { get; }
        public int Count => _resources.Count;

        public ExecutionPlan(
            IEnumerable<ExecutionPlanItem> resources,
            bool requiresAdministrator)
        {
            _resources = resources.ToReadOnlyList();
            RequiresAdministrator = requiresAdministrator;
        }

        public IEnumerator<ExecutionPlanItem> GetEnumerator()
        {
            return _resources.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
