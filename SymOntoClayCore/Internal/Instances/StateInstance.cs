using SymOntoClay.Core.Internal.CodeExecution;
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
        public StateInstance(StateDef codeItem, IEngineContext context, IStorage parentStorage, List<Var> varList)
            : base(codeItem, context, parentStorage, new StateStorageFactory(), varList)
        {
        }

        /// <inheritdoc/>
        public override KindOfInstance KindOfInstance => KindOfInstance.StateInstance;

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
        protected override void ExecutionCoordinator_OnFinished()
        {
            Task.Run(() => {
                OnStateInstanceFinished?.Invoke(this);
            });

            base.ExecutionCoordinator_OnFinished();
        }

        public event Action<StateInstance> OnStateInstanceFinished;

        /// <inheritdoc/>
        protected override void OnDisposed()
        {
#if DEBUG
            //Log($"Name = {Name}");
#endif

            if (_executionCoordinator.ExecutionStatus == ActionExecutionStatus.Executing)
            {
                _executionCoordinator.ExecutionStatus = ActionExecutionStatus.Canceled;
            }

            foreach (var stateDeactivator in _stateDeactivators)
            {
                stateDeactivator.Dispose();
            }

            base.OnDisposed();
        }
    }
}
