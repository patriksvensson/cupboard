namespace Cupboard.Testing;

[PublicAPI]
public sealed class FakeSecurityPrincipal : ISecurityPrincipal
{
    public bool IsAdministrator { get; }

    public FakeSecurityPrincipal(bool isAdministrator)
    {
        IsAdministrator = isAdministrator;
    }
}
