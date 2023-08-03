using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cupboard;

public sealed class Report : IEnumerable<ReportItem>
{
    public IReadOnlyList<ReportItem> Items { get; }
    public FactCollection Facts { get; }
    public bool RequiresAdministrator { get; }
    public bool DryRun { get; }
    public bool PendingReboot { get; }

    public int Count => Items.Count;
    public bool Successful { get; }

    public Report(IEnumerable<ReportItem> items, FactCollection facts, bool requiresAdministrator, bool dryRun, bool pendingReboot)
    {
        Items = items.ToReadOnlyList();
        Facts = facts ?? throw new ArgumentNullException(nameof(facts));
        RequiresAdministrator = requiresAdministrator;
        DryRun = dryRun;
        PendingReboot = pendingReboot;

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
