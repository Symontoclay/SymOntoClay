/*MIT License

Copyright (c) 2020 - 2024 Sergiy Tolkachov

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.*/

using NLog;
using SymOntoClay.Core.EventsInterfaces;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.Storage;
using SymOntoClay.Monitor.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.Core.Internal.Instances
{
    public class StateInstance : BaseIndependentInstance
    {
        public StateInstance(StateDef codeItem, IEngineContext context, IStorage parentStorage, ILocalCodeExecutionContext parentCodeExecutionContext, List<Var> varList)
            : base(codeItem, context, parentStorage, parentCodeExecutionContext, new StateStorageFactory(), varList)
        {
        }

        /// <inheritdoc/>
        public override KindOfInstance KindOfInstance => KindOfInstance.StateInstance;

        private List<StateDeactivator> _stateDeactivators = new List<StateDeactivator>();

        /// <inheritdoc/>
        protected override void RunDeactivatorsOfStates(IMonitorLogger logger)
        {
            if(_codeItem.DeactivatingConditions.Any())
            {
                foreach(var clause in _codeItem.DeactivatingConditions)
                {
                    var deactivatorInstance = new StateDeactivator(clause, this, _context, _storage, _localCodeExecutionContext);

                    _stateDeactivators.Add(deactivatorInstance);

                    deactivatorInstance.Init(logger);
                }
            }
        }

        /// <inheritdoc/>
        public override void ExecutionCoordinator_OnFinished()
        {
            EmitOnStateInstanceFinishedHandlers(this);

            base.ExecutionCoordinator_OnFinished();
        }

        public void AddOnStateInstanceFinishedHandler(IOnStateInstanceFinishedStateInstanceHandler handler)
        {
            lock(_onStateInstanceFinishedHandlersLockObj)
            {
                if(_onStateInstanceFinishedHandlers.Contains(handler))
                {
                    return;
                }

                _onStateInstanceFinishedHandlers.Add(handler);
            }
        }

        public void RemoveOnStateInstanceFinishedHandler(IOnStateInstanceFinishedStateInstanceHandler handler)
        {
            lock (_onStateInstanceFinishedHandlersLockObj)
            {
                if (_onStateInstanceFinishedHandlers.Contains(handler))
                {
                    _onStateInstanceFinishedHandlers.Remove(handler);
                }
            }
        }

        private void EmitOnStateInstanceFinishedHandlers(StateInstance value)
        {
            lock (_onStateInstanceFinishedHandlersLockObj)
            {
                foreach(var handler in _onStateInstanceFinishedHandlers)
                {
                    handler.Invoke(value);
                }
            }
        }

        private object _onStateInstanceFinishedHandlersLockObj = new object();
        private List<IOnStateInstanceFinishedStateInstanceHandler> _onStateInstanceFinishedHandlers = new List<IOnStateInstanceFinishedStateInstanceHandler>();

        /// <inheritdoc/>
        protected override void OnDisposed()
        {
            if (_executionCoordinator.ExecutionStatus == ActionExecutionStatus.Executing)
            {
                _executionCoordinator.SetExecutionStatus(Logger, "E5B59D5E-0F95-43EC-85A3-EFE74CDD327A", ActionExecutionStatus.Canceled);
            }

            foreach (var stateDeactivator in _stateDeactivators)
            {
                stateDeactivator.Dispose();
            }

            _onStateInstanceFinishedHandlers.Clear();

            base.OnDisposed();
        }
    }
}
