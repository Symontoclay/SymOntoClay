/*MIT License

Copyright (c) 2020 - 2023 Sergiy Tolkachov

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

using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.Core.Internal.Storage;
using SymOntoClay.CoreHelper;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.Core.Internal.Instances
{
    public class AppInstance : BaseIndependentInstance
    {
        public AppInstance(CodeItem codeItem, IEngineContext context, IStorage parentStorage)
            : base(codeItem, context, parentStorage, null, new ObjectStorageFactory(), null)
        {
            _statesResolver = _context.DataResolversFactory.GetStatesResolver();
        }
        
        /// <inheritdoc/>
        public override KindOfInstance KindOfInstance => KindOfInstance.AppInstance;

        private StatesResolver _statesResolver;

        private StrongIdentifierValue _stateNameForAutomaticStart;

        /// <inheritdoc/>
        public override IList<IInstance> GetTopIndependentInstances()
        {
            var result = new List<IInstance>() { this };

            lock(_stateLockObj)
            {
                if(_activeStatesDict.Any())
                {
                    throw new NotImplementedException();
                }
            }

            return result;
        }

        /// <inheritdoc/>
        protected override void ApplyCodeDirectives()
        {
            var codeItemDirectivesResolver = _context.DataResolversFactory.GetCodeItemDirectivesResolver();

            var directivesList = codeItemDirectivesResolver.Resolve(_localCodeExecutionContext);

            foreach (var directive in directivesList)
            {
                var kindOfDirective = directive.KindOfCodeItemDirective;

                switch (kindOfDirective)
                {
                    case KindOfCodeItemDirective.SetDefaultState:
                        {
                            var directiveItem = directive.AsSetDefaultStateDirective;

                            _storage.StatesStorage.SetDefaultStateName(directiveItem.StateName);

                            if(!directivesList.Any(p => p.KindOfCodeItemDirective == KindOfCodeItemDirective.SetState))
                            {
                                _stateNameForAutomaticStart = directiveItem.StateName;
                            }
                        }
                        break;

                    case KindOfCodeItemDirective.SetState:
                        {
                            var directiveItem = directive.AsSetStateDirective;

                            _stateNameForAutomaticStart = directiveItem.StateName;
                        }
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(kindOfDirective), kindOfDirective, null);
                }
            }

        }

        /// <inheritdoc/>
        protected override void RunMutuallyExclusiveStatesSets()
        {
            var itemsList = _statesResolver.ResolveMutuallyExclusiveStatesSetsList(_localCodeExecutionContext);

            if (itemsList.Any())
            {
                var targetDict = itemsList.SelectMany(p => p.StateNames).Distinct().ToDictionary(p => p, p => new List<StrongIdentifierValue>());

                foreach(var item in itemsList)
                {
                    var stateNamesList = item.StateNames;

                    foreach (var stateName in stateNamesList)
                    {
                        var exceptList = stateNamesList.ToList();
                        exceptList.Remove(stateName);

                        targetDict[stateName].AddRange(exceptList);
                    }
                }

                _mutuallyExclusiveStatesSet = targetDict.ToDictionary(p => p.Key, p => new HashSet<StrongIdentifierValue>(p.Value.Distinct()));
            }

        }

        /// <inheritdoc/>
        protected override void RunExplicitStates()
        {
            if(_stateNameForAutomaticStart == null)
            {
                return;
            }

            var state = _statesResolver.Resolve(_stateNameForAutomaticStart, _localCodeExecutionContext);

            ActivateState(state);

        }

        private Dictionary<StrongIdentifierValue, StateInstance> _activeStatesDict = new Dictionary<StrongIdentifierValue, StateInstance>();
        private Dictionary<StrongIdentifierValue, HashSet<StrongIdentifierValue>> _mutuallyExclusiveStatesSet = new Dictionary<StrongIdentifierValue, HashSet<StrongIdentifierValue>>();

        private readonly object _statesLockObj = new object();

        public bool IsStateActivated(StrongIdentifierValue stateName)
        {
            lock (_statesLockObj)
            {
                if (_activeStatesDict.ContainsKey(stateName))
                {
                    return true;
                }

                return false;
            }
        }

        public void ActivateState(StateDef state)
        {
            ActivateState(state, null);
        }

        public void ActivateState(StateDef state, List<Var> varList)
        {
            Task.Run(() => {
                StateInstance stateInstance = null;

                var statesForDeactivating = new List<StateInstance>();

                lock (_statesLockObj)
                {
                    var stateName = state.Name;

                    if (_activeStatesDict.ContainsKey(stateName))
                    {
                        return;
                    }

                    if(_mutuallyExclusiveStatesSet.ContainsKey(stateName))
                    {
                        var initialMutuallyExclusiveStatesSet = _mutuallyExclusiveStatesSet[stateName];

                        foreach(var nameItem in initialMutuallyExclusiveStatesSet)
                        {
                            if(_activeStatesDict.ContainsKey(nameItem))
                            {
                                statesForDeactivating.Add(_activeStatesDict[nameItem]);
                                _activeStatesDict.Remove(nameItem);
                            }
                        }               
                    }

                    stateInstance = new StateInstance(state, _context, _storage, _localCodeExecutionContext, varList);

                    _activeStatesDict[stateName] = stateInstance;

                    stateInstance.OnStateInstanceFinished += ChildStateInstance_OnFinished;
                }

                if (statesForDeactivating.Any())
                {
                    foreach(var stateForDeactivating in statesForDeactivating)
                    {
                        stateForDeactivating.Dispose();
                    }
                }

                stateInstance.Init();
            });
        }

        public void TryActivateDefaultState()
        {
            var defaultStateName = _statesResolver.ResolveDefaultStateName(_localCodeExecutionContext);

            if (defaultStateName != null)
            {
                var state = _statesResolver.Resolve(defaultStateName, _localCodeExecutionContext);

                ActivateState(state);
            }
        }

        private void ChildStateInstance_OnFinished(StateInstance stateInstance)
        {
            stateInstance.OnStateInstanceFinished -= ChildStateInstance_OnFinished;

            lock(_statesLockObj)
            {
                if(_activeStatesDict.ContainsKey(stateInstance.Name))
                {
                    _activeStatesDict.Remove(stateInstance.Name);
                }
            }
        }

        private List<StateActivator> _stateActivators = new List<StateActivator>();

        /// <inheritdoc/>
        protected override void RunActivatorsOfStates()
        {
            var activatorsInfoList = _statesResolver.ResolveActivationInfoOfStateList(_localCodeExecutionContext);

            if(!activatorsInfoList.Any())
            {
                return;
            }

            foreach(var activatorInfo in activatorsInfoList)
            {
                var activatorInstance = new StateActivator(activatorInfo, this, _context, _storage, _localCodeExecutionContext);

                _stateActivators.Add(activatorInstance);

                Task.Run(() => { activatorInstance.Init(); });
            }

        }

        /// <inheritdoc/>
        protected override void OnDisposed()
        {
            foreach(var stateActivator in _stateActivators)
            {
                stateActivator.Dispose();
            }

            base.OnDisposed();
        }
    }
}
