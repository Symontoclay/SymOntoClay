using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Monitor.Common;
using System.Collections.Generic;

namespace SymOntoClay.Core.Internal.Instances
{
    public interface IAppInstanceSerializedEventsHandler
    {
        void NActivateState(IMonitorLogger logger, StateDef state, List<VarInstance> varList);
        void NChildStateInstance_OnFinished(StateInstance stateInstance);
    }
}
