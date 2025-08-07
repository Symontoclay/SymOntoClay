using SymOntoClay.Core.Internal.CodeModel;
using System.Collections.Generic;

namespace SymOntoClay.Core.Internal.CodeExecution
{
    public class ValueListCallResult : CallResult<List<Value>>
    {
        public ValueListCallResult()
        {
        }

        public ValueListCallResult(List<Value> values)
            : base(values)
        {
        }
    }
}
