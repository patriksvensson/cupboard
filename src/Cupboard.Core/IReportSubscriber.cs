namespace Cupboard;

[PublicAPI]
public interface IReportSubscriber
{
    void Notify(Report report);
}
