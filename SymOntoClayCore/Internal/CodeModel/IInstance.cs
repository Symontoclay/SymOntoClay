using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.CoreHelper;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public interface IInstance: ISymOntoClayDisposable, IObjectToString, IObjectToShortString, IObjectToBriefString
    {
        StrongIdentifierValue Name { get; }
        IExecutionCoordinator ExecutionCoordinator { get; }
        void CancelExecution();
        void AddChildInstance(IInstance instance);
        void RemoveChildInstance(IInstance instance);
        void SetParent(IInstance instance);
        void ResetParent(IInstance instance);
    }
}
