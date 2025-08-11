using System;

namespace SymOntoClay.Core.Internal.DataResolvers
{
    [Flags]
    public enum KindOfValueConversion
    {
        Var = 1,
        Property = 2,
        ImplicitProperty = 4,
        LinVar = 8,
        All = 16,
        None = 32
    }
}
