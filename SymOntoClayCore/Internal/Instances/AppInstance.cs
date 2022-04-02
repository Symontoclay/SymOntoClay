/*MIT License

Copyright (c) 2020 - <curr_year/> Sergiy Tolkachov

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
    public class AppInstance : BaseInstance
    {
        public AppInstance(CodeItem codeItem, IEngineContext context, IStorage parentStorage)
            : base(codeItem, context, parentStorage, new ObjectStorageFactory(), null)
        {
            _statesResolver = _context.DataResolversFactory.GetStatesResolver();
        }

        /// <inheritdoc/>
        public override KindOfInstance KindOfInstance => KindOfInstance.AppInstance;

        private StatesResolver _statesResolver;

        private StrongIdentifierValue _stateNameForAutomaticStart;

        /// <inheritdoc/>
        protected override void ApplyCodeDirectives()
        {
#if DEBUG
            //Log("Begin");
#endif

            var codeItemDirectivesResolver = _context.DataResolversFactory.GetCodeItemDirectivesResolver();

            var directivesList = codeItemDirectivesResolver.Resolve(_localCodeExecutionContext);

#if DEBUG
            //Log($"_codeItem = {_codeItem}");
#endif

            foreach (var directive in directivesList)
            {
#if DEBUG
                //Log($"directive = {directive}");
#endif

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

#if DEBUG
            //Log("End");
#endif
        }

        /// <inheritdoc/>
        protected override void RunMutuallyExclusiveStatesSets()
        {
#if DEBUG
            //Log("Begin");
#endif

            var itemsList = _statesResolver.ResolveMutuallyExclusiveStatesSetsList(_localCodeExecutionContext);

#if DEBUG
            //Log($"itemsList.Count = {itemsList.Count}");
            //Log($"itemsList = {itemsList.WriteListToString()}");
#endif

            if (itemsList.Any())
            {
                var targetDict = itemsList.SelectMany(p => p.StateNames).Distinct().ToDictionary(p => p, p => new List<StrongIdentifierValue>());

                foreach(var item in itemsList)
                {
#if DEBUG
                    //Log($"item = {item.ToHumanizedString()}");
#endif

                    var stateNamesList = item.StateNames;

                    foreach (var stateName in stateNamesList)
                    {
#if DEBUG
                        //Log($"stateName = {stateName.ToHumanizedString()}");
#endif

                        var exceptList = stateNamesList.ToList();
                        exceptList.Remove(stateName);

#if DEBUG
                        //Log($"exceptList = {exceptList.WriteListToString()}");
#endif

                        targetDict[stateName].AddRange(exceptList);
                    }
                }

                _mutuallyExclusiveStatesSet = targetDict.ToDictionary(p => p.Key, p => new HashSet<StrongIdentifierValue>(p.Value.Distinct()));
            }

#if DEBUG
            //Log("End");
#endif
        }

        /// <inheritdoc/>
        protected override void RunExplicitStates()
        {
#if DEBUG
            //Log("Begin");
#endif

#if DEBUG
            //Log($"_stateNameForAutomaticStart = {_stateNameForAutomaticStart}");
#endif

            if(_stateNameForAutomaticStart == null)
            {
                return;
            }

            var state = _statesResolver.Resolve(_stateNameForAutomaticStart, _localCodeExecutionContext);

#if DEBUG
            //Log($"state = {state}");
#endif

            ActivateState(state);

#if DEBUG
            //Log("End");
#endif
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
#if DEBUG
                    //Log($"state = {state}");
#endif

                    var stateName = state.Name;

                    if (_activeStatesDict.ContainsKey(stateName))
                    {
#if DEBUG
                        //Log("_activeStatesDict.ContainsKey(stateName) return;");
#endif

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

                    stateInstance = new StateInstance(state, _context, _storage, varList);

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

#if DEBUG
            //Log($"defaultStateName = {defaultStateName}");
#endif
            
            if (defaultStateName != null)
            {
                var state = _statesResolver.Resolve(defaultStateName, _localCodeExecutionContext);

#if DEBUG
                //Log($"state = {state}");
#endif

                ActivateState(state);
            }
        }

        private void ChildStateInstance_OnFinished(StateInstance stateInstance)
        {
#if DEBUG
            //Log($"stateInstance = {stateInstance}");
#endif

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
#if DEBUG
            //Log("Begin");
#endif

            var activatorsInfoList = _statesResolver.ResolveActivationInfoOfStateList(_localCodeExecutionContext);

#if DEBUG
            //Log($"activatorsInfoList = {activatorsInfoList.WriteListToString()}");
#endif

            if(!activatorsInfoList.Any())
            {
                return;
            }

            foreach(var activatorInfo in activatorsInfoList)
            {
#if DEBUG
                //Log($"activatorInfo = {activatorInfo}");
#endif

                var activatorInstance = new StateActivator(activatorInfo, this, _context, _storage);

                _stateActivators.Add(activatorInstance);

                Task.Run(() => { activatorInstance.Init(); });
            }

#if DEBUG
            //Log("End");
#endif
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
