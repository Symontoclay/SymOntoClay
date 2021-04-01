using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeExecution
{
    public interface IFuzzyLogicMemberFunctionHandler: IObjectToString, IObjectToShortString, IObjectToBriefString
    {
        KindOfFuzzyLogicMemberFunction Kind { get; }
        double SystemCall(NumberValue x);
        double SystemCall(double x);
        ulong GetLongHashCode();
    }
}
