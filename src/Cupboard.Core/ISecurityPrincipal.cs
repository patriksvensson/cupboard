namespace Cupboard;

public interface ISecurityPrincipal
{
    bool IsAdministrator { get; }
}
