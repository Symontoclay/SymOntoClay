using SymOntoClay.Core.Internal.IndexedData;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeExecution
{
    public interface IUnaryOperatorHandler
    {
        IndexedValue Call(IndexedValue operand, IndexedValue annotation, LocalCodeExecutionContext localCodeExecutionContext);
    }
}
