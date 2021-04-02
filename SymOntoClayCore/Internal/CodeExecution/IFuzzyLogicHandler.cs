using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeExecution
{
    public interface IFuzzyLogicHandler : IObjectToString, IObjectToShortString, IObjectToBriefString
    {
        double SystemCall(NumberValue x);
        double SystemCall(double x);
        ulong GetLongHashCode();
    }
}
