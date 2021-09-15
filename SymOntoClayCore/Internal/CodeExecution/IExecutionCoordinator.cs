using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeExecution
{
    public interface IExecutionCoordinator : IObjectToString, IObjectToShortString, IObjectToBriefString
    {
        ActionExecutionStatus ExecutionStatus { get; set; }
        RuleInstance RuleInstance { get; set; }
    }
}
