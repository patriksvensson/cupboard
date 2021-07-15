using System.Collections;
using System.Collections.Generic;

namespace Cupboard.Internal
{
    internal sealed class ExecutionPlan : IEnumerable<ExecutionPlanItem>
    {
        private readonly IReadOnlyList<ExecutionPlanItem> _resources;

        public int Count => _resources.Count;

        public ExecutionPlan(IEnumerable<ExecutionPlanItem> resources)
        {
            _resources = resources.ToReadOnlyList();
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
