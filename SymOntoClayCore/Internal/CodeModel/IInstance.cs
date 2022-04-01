using SymOntoClay.Core.Internal.CodeExecution;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public interface IInstance
    {
        IExecutionCoordinator ExecutionCoordinator { get; }
    }
}
