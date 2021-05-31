using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeExecution
{
    public interface IFunctionArgument: IObjectToString, IObjectToShortString, IObjectToBriefString
    {
        StrongIdentifierValue Name { get; }
        IList<StrongIdentifierValue> TypesList { get; }
        bool HasDefaultValue { get; }
        Value DefaultValue { get; }
    }
}
