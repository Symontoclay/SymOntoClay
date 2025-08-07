using SymOntoClay.Core.Internal.CodeModel;

namespace SymOntoClay.Core.Internal.CodeExecution
{
    public class ValueCallResult: CallResult<Value>
    {
        public ValueCallResult()
        {
        }

        public ValueCallResult(Value value)
            : base(value)
        {
        }
    }
}
