using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.IndexedData
{
    public interface IResultOfVarOfQueryToRelation : IObjectWithParametrizedLongHashCode, IObjectToString, IObjectToShortString, IObjectToBriefString, IObjectToHumanizedString
    {
        StrongIdentifierValue NameOfVar { get; set; }
        LogicalQueryNode FoundExpression { get; set; }
    }
}
