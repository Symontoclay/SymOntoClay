using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Instances
{
    public class StateActivator : BaseTriggerInstance
    {
        public StateActivator(ActivationInfoOfStateDef activationInfoOfState, AppInstance parent, IEngineContext context, IStorage parentStorage)
            : base(activationInfoOfState.ActivatingConditions, parent, context, parentStorage)
        {
            _stateDef = activationInfoOfState.State;
            _stateName = _stateDef.Name;
            _appInstance = parent;
        }

        private StateDef _stateDef;
        private AppInstance _appInstance;
        private StrongIdentifierValue _stateName;

        /// <inheritdoc/>
        protected override bool ShouldSearch()
        {
#if DEBUG
            //Log($"_stateName = {_stateName}");
#endif

            return !_appInstance.IsStateActivated(_stateName);
        }

        /// <inheritdoc/>
        protected override void RunHandler(LocalCodeExecutionContext localCodeExecutionContext)
        {
#if DEBUG
            //Log("Begin");
#endif

            _appInstance.ActivateState(_stateDef);
        }
    }
}
