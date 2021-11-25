namespace Cupboard;

public interface IReportSubscriber
{
    void Notify(Report report);
}