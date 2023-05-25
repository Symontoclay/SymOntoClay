using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.IndexedData
{
    public interface IResultOfQueryToRelation : IObjectToString, IObjectToShortString, IObjectToBriefString
    {
        IList<IResultOfVarOfQueryToRelation> ResultOfVarOfQueryToRelationList { get; set; }
    }
}
