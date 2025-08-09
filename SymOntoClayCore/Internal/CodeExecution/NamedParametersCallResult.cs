using SymOntoClay.Core.Internal.CodeModel;
using System.Collections.Generic;

namespace SymOntoClay.Core.Internal.CodeExecution
{
    public class NamedParametersCallResult : CallResult<Dictionary<StrongIdentifierValue, Value>>
    {
        public NamedParametersCallResult()
        {
        }

        public NamedParametersCallResult(Dictionary<StrongIdentifierValue, Value> values)
            : base(values)
        {
        }
    }
}
