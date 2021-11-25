using System;

namespace Cupboard;

[Obsolete("Please use RegistryValueKind instead")]
public enum RegistryKeyValueKind
{
    None = -1,
    Unknown = 0,
    String = 1,
    ExpandString = 2,
    Binary = 3,
    DWord = 4,
    MultiString = 7,
    QWord = 11,
}