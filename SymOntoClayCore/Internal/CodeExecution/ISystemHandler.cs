using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeExecution
{
    public interface ISystemHandler
    {
        Value Call(IList<Value> paramsList,  IStorage localContext);
        Value Call(IDictionary<ulong, Value> paramsDict, IStorage localContext);
    }
}
