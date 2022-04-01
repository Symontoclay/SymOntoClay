using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public interface IInstance: IObjectToString, IObjectToShortString, IObjectToBriefString
    {
        StrongIdentifierValue Name { get; }
        IExecutionCoordinator ExecutionCoordinator { get; }
    }
}
