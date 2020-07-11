using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeExecution
{
    public interface IBinaryOperatorHandler
    {
        Value Call(Value leftOperand, Value rightOperand, LocalCodeExecutionContext localCodeExecutionContext);
    }
}
