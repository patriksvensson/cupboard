using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cupboard.Internal;

namespace Cupboard
{
    public sealed class Report : IEnumerable<ReportItem>
    {
        public IReadOnlyList<ReportItem> Items { get; }
        public int Count => Items.Count;
        public bool Successful { get; }
        public bool DryRun { get; }

        public Report(IEnumerable<ReportItem> items, bool dryRun = false)
        {
            Items = items.ToReadOnlyList();
            DryRun = dryRun;

            // Exclude Unknown states if this is a dry run
            var temp = DryRun ? Items.Where(x => x.State != ResourceState.Unknown) : Items;
            Successful = temp.All(x => !x.State.IsError());
        }

        public IEnumerator<ReportItem> GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
