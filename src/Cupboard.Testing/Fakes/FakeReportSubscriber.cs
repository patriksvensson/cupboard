namespace Cupboard.Testing;

internal sealed class FakeReportSubscriber : IReportSubscriber
{
    public Report? Report { get; set; }

    public void Notify(Report report)
    {
        Report = report;
    }
}