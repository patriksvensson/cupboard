namespace Cupboard
{
    public sealed class RegistryKey : Resource
    {
        public RegistryKeyPath? Path { get; set; }
        public object? Value { get; set; }
        public RegistryKeyValueKind ValueKind { get; set; } = RegistryKeyValueKind.Unknown;
        public RegistryKeyState State { get; set; } = RegistryKeyState.Exist;

        public RegistryKey(string name)
            : base(name)
        {
        }
    }
}
