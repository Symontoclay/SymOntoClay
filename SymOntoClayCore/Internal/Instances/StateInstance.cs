﻿using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.Storage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.Core.Internal.Instances
{
    public class StateInstance : BaseInstance
    {
        public StateInstance(StateDef codeItem, IEngineContext context, IStorage parentStorage, IExecutionCoordinator appInstanceExecutionCoordinator)
            : base(codeItem, context, parentStorage, new StateStorageFactory())
        {
            _appInstanceExecutionCoordinator = appInstanceExecutionCoordinator;
        }

        /// <inheritdoc/>
        protected override void InitExecutionCoordinators()
        {
            _stateExecutionCoordinator = new ExecutionCoordinator();
            _stateExecutionCoordinator.OnFinished += StateExecutionCoordinator_OnFinished;
        }

        /// <inheritdoc/>
        protected override void SetExecutionStatusOfExecutionCoordinatorAsExecuting()
        {
            _stateExecutionCoordinator.ExecutionStatus = ActionExecutionStatus.Executing;
        }

        /// <inheritdoc/>
        protected override void RunInitialTriggers()
        {
            RunInitialTriggers(KindOfSystemEventOfInlineTrigger.Enter);
        }

        /// <inheritdoc/>
        protected override void StateExecutionCoordinator_OnFinished()
        {
            Task.Run(() => {
                OnStateInstanceFinished?.Invoke(this);
            });

            base.StateExecutionCoordinator_OnFinished();
        }

        public event Action<StateInstance> OnStateInstanceFinished;
    }
}