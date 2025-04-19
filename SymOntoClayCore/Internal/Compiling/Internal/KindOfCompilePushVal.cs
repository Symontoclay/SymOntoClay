using System;

namespace SymOntoClay.Core.Internal.Compiling.Internal
{
    [Flags]
    public enum KindOfCompilePushVal
    {
        Unknown = 1,
        DirectAllCases = 2,
        SetAllCases = 4,
        GetAllCases = 8,
        DirectOther = 16,
        SetOther = 32,
        GetOther = 64,
        DirectVar = 128,
        SetVar = 256,
        GetVar = 512,
        DirectProp = 1024,
        SetProp = 2048,
        GetProp = 4096
    }
}
