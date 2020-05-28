using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public interface IFuzzyExpr: IObjectToString, IObjectToShortString, IObjectToBriefString
    {
        Value Deffuzzificate(IStorage execContext);
    }
}
