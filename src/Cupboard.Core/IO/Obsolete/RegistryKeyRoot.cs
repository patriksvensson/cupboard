using System;

namespace Cupboard;

[Obsolete("Please use RegistryRoot instead")]
public enum RegistryKeyRoot
{
    Unknown = 0,
    ClassesRoot = 1,
    CurrentUser = 2,
    LocalMachine = 3,
    Users = 4,
    CurrentConfig = 5,
    PerformanceData = 6,
}