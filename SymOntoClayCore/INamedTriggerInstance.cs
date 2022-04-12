using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core
{
    public interface INamedTriggerInstance: IObjectWithLongHashCodes
    {
        IList<StrongIdentifierValue> NamesList { get; }
        bool IsOn { get; }
        event Action<IList<StrongIdentifierValue>> OnChanged;
    }
}
