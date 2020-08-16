using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core
{
    public interface ICommand : IObjectToString, IObjectToShortString, IObjectToBriefString
    {
        StrongIdentifierValue Name { get; }
        IList<Value> ParamsList { get; }
        IDictionary<StrongIdentifierValue, Value> ParamsDict { get; }
        int ParamsCount { get; }
        KindOfCommandParameters KindOfCommandParameters { get; }
    }
}
