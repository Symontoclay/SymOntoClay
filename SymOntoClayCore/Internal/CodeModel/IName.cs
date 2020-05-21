using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public interface IName : IObjectToString, IObjectToShortString, IObjectToBriefString
    {
        string OriginalValue { get; }
        ulong OriginalKey { get; }
        ulong Key { get; }
    }
}
