﻿using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.Core.Internal.Instances
{
    public class StateInstance : BaseInstance
    {
        public StateInstance(StateDef codeItem, IEngineContext context, IStorage parentStorage, List<Var> varList, IExecutionCoordinator appInstanceExecutionCoordinator)
            : base(codeItem, context, parentStorage, new StateStorageFactory(), varList)
        {
            _appInstanceExecutionCoordinator = appInstanceExecutionCoordinator;
        }

        /// <inheritdoc/>
        protected override void InitExecutionCoordinators()
        {
            _stateExecutionCoordinator = new ExecutionCoordinator(this);
            _stateExecutionCoordinator.OnFinished += StateExecutionCoordinator_OnFinished;
        }

        /// <inheritdoc/>
        public override IExecutionCoordinator ExecutionCoordinator => _stateExecutionCoordinator;

        /// <inheritdoc/>
        protected override void SetExecutionStatusOfExecutionCoordinatorAsExecuting()
        {
            _stateExecutionCoordinator.ExecutionStatus = ActionExecutionStatus.Executing;
        }

        private List<StateDeactivator> _stateDeactivators = new List<StateDeactivator>();

        /// <inheritdoc/>
        protected override void RunDeactivatorsOfStates()
        {
            if(_codeItem.DeactivatingConditions.Any())
            {
                foreach(var clause in _codeItem.DeactivatingConditions)
                {
                    var deactivatorInstance = new StateDeactivator(clause, this, _context, _storage);

                    _stateDeactivators.Add(deactivatorInstance);

                    deactivatorInstance.Init();
                }
            }
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

        /// <inheritdoc/>
        protected override void InitFinalizationExecutionCoordinators()
        {
            _appInstanceFinalizationExecutionCoordinator = _appInstanceExecutionCoordinator;
            _stateFinalizationExecutionCoordinator = new ExecutionCoordinator(this);
            _stateFinalizationExecutionCoordinator.ExecutionStatus = ActionExecutionStatus.Executing;
        }

        /// <inheritdoc/>
        protected override void OnDisposed()
        {
#if DEBUG
            //Log($"Name = {Name}");
#endif

            if (_stateExecutionCoordinator.ExecutionStatus == ActionExecutionStatus.Executing)
            {
                _stateExecutionCoordinator.ExecutionStatus = ActionExecutionStatus.Canceled;
            }

            foreach (var stateDeactivator in _stateDeactivators)
            {
                stateDeactivator.Dispose();
            }

            base.OnDisposed();
        }
    }
}
