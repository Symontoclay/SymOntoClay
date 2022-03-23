using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Instances
{
    public class StateActivator : BaseTriggerInstance
    {
        public StateActivator(ActivationInfoOfStateDef activationInfoOfState, BaseInstance parent, IEngineContext context, IStorage parentStorage)
            : base(activationInfoOfState.ActivatingClause, parent, context, parentStorage)
        {
            _activationInfoOfState = activationInfoOfState;
        }

        private StateDef _stateDef;
    }
}
