namespace Cupboard;

[PublicAPI]
public interface ISecurityPrincipal
{
    bool IsAdministrator { get; }
}
