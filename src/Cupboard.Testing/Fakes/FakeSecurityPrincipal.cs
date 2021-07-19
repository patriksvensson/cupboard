namespace Cupboard.Testing
{
    public sealed class FakeSecurityPrincipal : ISecurityPrincipal
    {
        public bool IsAdmin { get; set; }

        public bool IsAdministrator()
        {
            return IsAdmin;
        }
    }
}
