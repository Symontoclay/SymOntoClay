/*MIT License

Copyright (c) 2020 - 2022 Sergiy Tolkachov

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

using Newtonsoft.Json;
using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.Core.Internal.Instances.LogicConditionalTriggerExecutors;
using SymOntoClay.Core.Internal.Instances.LogicConditionalTriggerObservers;
using SymOntoClay.Core.Internal.Storage;
using SymOntoClay.Core.Internal.Threads;
using SymOntoClay.CoreHelper;
using SymOntoClay.CoreHelper.CollectionsHelpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.Core.Internal.Instances
{
    public class LogicConditionalTriggerInstance : BaseComponent, INamedTriggerInstance, IObjectToString, IObjectToShortString, IObjectToBriefString
    {
        public LogicConditionalTriggerInstance(InlineTrigger trigger, BaseInstance parent, IEngineContext context, IStorage parentStorage, LocalCodeExecutionContext parentCodeExecutionContext)
            : base(context.Logger)
        {
            _executionCoordinator = parent.ExecutionCoordinator;
            _context = context;
            _globalLogicalStorage = context.Storage.GlobalStorage.LogicalStorage;
            _parent = parent;
            _trigger = trigger;
            _namesList = trigger.NamesList;
            _hasNames = !_namesList.IsNullOrEmpty();
            _dateTimeProvider = _context.DateTimeProvider;

#if DEBUG
            //_trigger.DoubleConditionsStrategy = DoubleConditionsStrategy.PriorReset;
            //_trigger.DoubleConditionsStrategy = DoubleConditionsStrategy.Equal;
            //Log($"_trigger = {_trigger}");
            //Log($"_trigger.SetCondition = {_trigger.SetCondition?.GetHumanizeDbgString()}");
            //Log($"_trigger.ResetCondition = {_trigger.ResetCondition?.GetHumanizeDbgString()}");
            //Log($"_trigger.DoubleConditionsStrategy = {_trigger.DoubleConditionsStrategy}");
            //Log($"_dateTimeProvider.CurrentTiks = {_dateTimeProvider.CurrentTiks}");
#endif

            _localCodeExecutionContext = new LocalCodeExecutionContext(parentCodeExecutionContext);
            var localStorageSettings = RealStorageSettingsHelper.Create(context, parentStorage);
            _storage = new LocalStorage(localStorageSettings);
            _localCodeExecutionContext.Storage = _storage;

            _localCodeExecutionContext.Holder = parent.Name;

            _triggerConditionNodeObserverContext = new TriggerConditionNodeObserverContext(context, _storage, parent.Name);

            _setConditionalTriggerObserver = new LogicConditionalTriggerObserver(_triggerConditionNodeObserverContext, trigger.SetCondition);
            _setConditionalTriggerObserver.OnChanged += Observer_OnChanged;

            _ruleInstancesList = _trigger.RuleInstancesList;

            _hasRuleInstancesList = !_ruleInstancesList.IsNullOrEmpty();

            var setBindingVariables = trigger.SetBindingVariables;

            if(_hasRuleInstancesList)
            {
                setBindingVariables = new BindingVariables();
            }

            _setConditionalTriggerExecutor = new LogicConditionalTriggerExecutor(_triggerConditionNodeObserverContext,  trigger.SetCondition, trigger.SetBindingVariables);

            _hasResetHandler = trigger.ResetCompiledFunctionBody != null;

            if (_trigger.ResetCondition != null)
            {
                _hasResetConditions = true;

                _resetConditionalTriggerObserver = new LogicConditionalTriggerObserver(_triggerConditionNodeObserverContext, trigger.ResetCondition);
                _resetConditionalTriggerObserver.OnChanged += Observer_OnChanged;

                var resetBindingVariables = _trigger.ResetBindingVariables;

                if(resetBindingVariables == null)
                {
                    resetBindingVariables = new BindingVariables();
                }

                if (_hasRuleInstancesList)
                {
                    resetBindingVariables = new BindingVariables();
                }

                _resetConditionalTriggerExecutor = new LogicConditionalTriggerExecutor(_triggerConditionNodeObserverContext, trigger.ResetCondition, resetBindingVariables);
            }
        }

        private readonly TriggerConditionNodeObserverContext _triggerConditionNodeObserverContext;
        private readonly IDateTimeProvider _dateTimeProvider;

        private readonly IExecutionCoordinator _executionCoordinator;
        private readonly IEngineContext _context;
        private readonly ILogicalStorage _globalLogicalStorage;
        private readonly BaseInstance _parent;
        private readonly InlineTrigger _trigger;
        private readonly IList<StrongIdentifierValue> _namesList;
        private readonly bool _hasNames;
        private readonly List<RuleInstance> _ruleInstancesList;
        private readonly bool _hasRuleInstancesList;
        private readonly IStorage _storage;
        private readonly LocalCodeExecutionContext _localCodeExecutionContext;
        private readonly LogicConditionalTriggerObserver _setConditionalTriggerObserver;
        private readonly LogicConditionalTriggerObserver _resetConditionalTriggerObserver;
        private readonly LogicConditionalTriggerExecutor _setConditionalTriggerExecutor;
        private readonly LogicConditionalTriggerExecutor _resetConditionalTriggerExecutor;

        private readonly object _lockObj = new object();
        private bool _isBusy;
        private bool _needRepeat;

        private bool _isOn;

        private readonly bool _hasResetConditions;
        private readonly bool _hasResetHandler;

        /// <inheritdoc/>
        public IList<StrongIdentifierValue> NamesList => _namesList;

        /// <inheritdoc/>
        public bool IsOn => _isOn;

        /// <inheritdoc/>
        public ulong GetLongHashCode()
        {
            return _trigger.GetLongHashCode();
        }

        /// <inheritdoc/>
        public ulong GetLongConditionalHashCode()
        {
            return _trigger.GetLongConditionalHashCode();
        }

        /// <inheritdoc/>
        public event Action<IList<StrongIdentifierValue>> OnChanged;

        public void Init()
        {
#if DEBUG
            //Log("Begin");
#endif

            Observer_OnChanged();

#if DEBUG
            //Log("End");
#endif
        }

        private void Observer_OnChanged()
        {
            Task.Run(() =>
            {
#if DEBUG
                //Log("Begin");
#endif

                try
                {
                    lock (_lockObj)
                    {
                        if (_isBusy)
                        {
                            _needRepeat = true;
                            return;
                        }

                        _isBusy = true;
                        _needRepeat = false;
                    }

                    DoSearch();

                    while (true)
                    {
                        lock (_lockObj)
                        {
                            if (!_needRepeat)
                            {
                                _isBusy = false;
                                return;
                            }

                            _needRepeat = false;
                        }

                        DoSearch();
                    }
                }
                catch (Exception e)
                {
                    Error(e);
                }
            });

#if DEBUG
            //Log("End");
#endif
        }

        private void DoSearch()
        {
#if DEBUG
            //Log("Begin");
#endif

#if DEBUG
            //Log($"_dateTimeProvider.CurrentTiks = {_dateTimeProvider.CurrentTiks}");
            //Log($"_isOn = {_isOn}");
#endif

            var oldIsOn = _isOn;

            if (_hasResetConditions)
            {
                switch(_trigger.DoubleConditionsStrategy)
                {
                    case DoubleConditionsStrategy.Equal:
                        DoSearchWithEqualConditions();
                        break;

                    case DoubleConditionsStrategy.PriorSet:
                        DoSearchWithPriorSetCondition();
                        break;

                    case DoubleConditionsStrategy.PriorReset:
                        DoSearchWithPriorResetCondition();
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(_trigger.DoubleConditionsStrategy), _trigger.DoubleConditionsStrategy, null);
                }
            }
            else
            {
                DoSearchWithNoResetCondition();
            }

#if DEBUG
            //Log($"_isOn (after) = {_isOn}");
#endif

            if(_isOn)
            {
                if(!_triggerConditionNodeObserverContext.SetSeconds.HasValue)
                {
                    _triggerConditionNodeObserverContext.SetSeconds = Convert.ToInt64(_dateTimeProvider.CurrentTiks * _dateTimeProvider.SecondsMultiplicator);
                }
            }
            else
            {
                if(_triggerConditionNodeObserverContext.SetSeconds.HasValue)
                {
                    _triggerConditionNodeObserverContext.SetSeconds = null;
                }
            }

            if(_hasRuleInstancesList && oldIsOn != _isOn)
            {
                if(_isOn)
                {
                    _globalLogicalStorage.Append(_ruleInstancesList);
                }
                else
                {
                    _globalLogicalStorage.Remove(_ruleInstancesList);
                }
            }

            if(_hasNames && oldIsOn != _isOn)
            {
                Task.Run(() => {
                    OnChanged?.Invoke(_namesList);
                });
            }

#if DEBUG
            //Log("End");
#endif
        }

        private void DoSearchWithEqualConditions()
        {
#if DEBUG
            //Log("Begin");
#endif

            var isSetSuccsess = _setConditionalTriggerExecutor.Run(out List<List<Var>> setVarList);

#if DEBUG
            //Log($"isSetSuccsess = {isSetSuccsess}");
            //Log($"setVarList.Count = {setVarList.Count}");
            //Log($"_setFoundKeys.Count = {_setFoundKeys.Count}");
#endif

            if (isSetSuccsess)
            {
                if (setVarList.Any())
                {
                    ProcessSetResultWithItems(setVarList);
                }
                else
                {
                    ProcessSetResultWithNoItems();
                }
            }

            var isResetSuccsess = _resetConditionalTriggerExecutor.Run(out List<List<Var>> resetVarList);

#if DEBUG
            //Log($"isResetSuccsess = {isResetSuccsess}");
            //Log($"resetVarList.Count = {resetVarList.Count}");
            //Log($"_resetFoundKeys.Count = {_resetFoundKeys.Count}");
#endif

            if (isResetSuccsess)
            {
                if (_hasResetHandler)
                {
                    if (resetVarList.Any())
                    {
                        ProcessResetResultWithItems(resetVarList);
                    }
                    else
                    {
                        ProcessResetResultWithNoItems();
                    }
                }
                else
                {
                    _isOn = false;
                }
            }

#if DEBUG
            //Log("End");
#endif
        }

        private void DoSearchWithPriorSetCondition()
        {
#if DEBUG
            //Log("Begin");
#endif

            var isSetSuccsess = _setConditionalTriggerExecutor.Run(out List<List<Var>> setVarList);

#if DEBUG
            //Log($"isSetSuccsess = {isSetSuccsess}");
            //Log($"setVarList.Count = {setVarList.Count}");
            //Log($"_setFoundKeys.Count = {_setFoundKeys.Count}");
#endif

            if (isSetSuccsess)
            {
                if (setVarList.Any())
                {
                    ProcessSetResultWithItems(setVarList);
                }
                else
                {
                    ProcessSetResultWithNoItems();
                }
            }
            else
            {
                RunResetCondition();
            }

#if DEBUG
            //Log("End");
#endif
        }

        private void DoSearchWithPriorResetCondition()
        {
#if DEBUG
            //Log("Begin");
#endif

            var isResetSuccsess = _resetConditionalTriggerExecutor.Run(out List<List<Var>> resetVarList);

#if DEBUG
            //Log($"isResetSuccsess = {isResetSuccsess}");
            //Log($"resetVarList.Count = {resetVarList.Count}");
            //Log($"_resetFoundKeys.Count = {_resetFoundKeys.Count}");
#endif

            if(isResetSuccsess)
            {
                if (_hasResetHandler)
                {
                    if (resetVarList.Any())
                    {
                        ProcessResetResultWithItems(resetVarList);
                    }
                    else
                    {
                        ProcessResetResultWithNoItems();
                    }
                }
            }
            else
            {
                if(!_isOn)
                {
                    var isSetSuccsess = _setConditionalTriggerExecutor.Run(out List<List<Var>> setVarList);

#if DEBUG
                    //Log($"isSetSuccsess = {isSetSuccsess}");
                    //Log($"setVarList.Count = {setVarList.Count}");
                    //Log($"_setFoundKeys.Count = {_setFoundKeys.Count}");
#endif

                    if(isSetSuccsess)
                    {
                        if (setVarList.Any())
                        {
                            ProcessSetResultWithItems(setVarList);
                        }
                        else
                        {
                            ProcessSetResultWithNoItems();
                        }
                    }
                }
            }

#if DEBUG
            //Log("End");
#endif
        }

        private void RunResetCondition()
        {
#if DEBUG
            //Log("Begin");
#endif

            //if (!_isOn)
            //{
            //    return;
            //}

            var isResetSuccsess = _resetConditionalTriggerExecutor.Run(out List<List<Var>> resetVarList);

#if DEBUG
            //Log($"isResetSuccsess = {isResetSuccsess}");
            //Log($"resetVarList.Count = {resetVarList.Count}");
            //Log($"_resetFoundKeys.Count = {_resetFoundKeys.Count}");
#endif

            if (isResetSuccsess)
            {
                if (_hasResetHandler)
                {
                    if (resetVarList.Any())
                    {
                        ProcessResetResultWithItems(resetVarList);
                    }
                    else
                    {
                        ProcessResetResultWithNoItems();
                    }
                }
            }

#if DEBUG
            //Log("End");
#endif
        }

        private void DoSearchWithNoResetCondition()
        {
#if DEBUG
            //Log("Begin");
#endif

            var isSetSuccsess = _setConditionalTriggerExecutor.Run(out List<List<Var>> setVarList);

#if DEBUG
            //Log($"isSetSuccsess = {isSetSuccsess}");
            //Log($"setVarList.Count = {setVarList.Count}");
#endif

            if (isSetSuccsess)
            {
                if (setVarList.Any())
                {
                    ProcessSetResultWithItems(setVarList);
                }
                else
                {
                    ProcessSetResultWithNoItems();
                }
            }
            else
            {
#if DEBUG
                //Log($"_hasResetHandler = {_hasResetHandler}");
#endif

                if (_hasResetHandler)
                {
                    ProcessResetResultWithNoItems();
                }

                _isOn = false;
            }

#if DEBUG
            //Log("End");
#endif
        }

        private void ProcessSetResultWithNoItems()
        {
#if DEBUG
            //Log("Begin");
            //Log($"_isOn = {_isOn}");
#endif

            if (_isOn)
            {
                return;
            }

            _isOn = true;

            if(_hasRuleInstancesList)
            {
                return;
            }

            var localCodeExecutionContext = new LocalCodeExecutionContext();
            var localStorageSettings = RealStorageSettingsHelper.Create(_context, _storage);
            var storage = new LocalStorage(localStorageSettings);
            localCodeExecutionContext.Storage = storage;
            localCodeExecutionContext.Holder = _parent.Name;

            RunSetHandler(localCodeExecutionContext);

#if DEBUG
            //Log("End");
#endif
        }

        private void ProcessSetResultWithItems(List<List<Var>> varList)
        {
#if DEBUG
            //Log("Begin");
#endif

            _isOn = true;

            if (_hasRuleInstancesList)
            {
                return;
            }

            foreach (var targetVarList in varList)
            {
                var localCodeExecutionContext = new LocalCodeExecutionContext();
                var localStorageSettings = RealStorageSettingsHelper.Create(_context, _storage);
                var storage = new LocalStorage(localStorageSettings);
                localCodeExecutionContext.Storage = storage;
                localCodeExecutionContext.Holder = _parent.Name;

                var varStorage = storage.VarStorage;

                foreach (var varItem in targetVarList)
                {
                    varStorage.Append(varItem);
                }

                RunSetHandler(localCodeExecutionContext);
            }

#if DEBUG
            //Log("End");
#endif
        }

        private void ProcessResetResultWithNoItems()
        {
#if DEBUG
            //Log("Begin");
#endif

            if(!_isOn)
            {
                return;
            }

            _isOn = false;

            if (_hasRuleInstancesList)
            {
                return;
            }

            var localCodeExecutionContext = new LocalCodeExecutionContext();
            var localStorageSettings = RealStorageSettingsHelper.Create(_context, _storage);
            var storage = new LocalStorage(localStorageSettings);
            localCodeExecutionContext.Storage = storage;
            localCodeExecutionContext.Holder = _parent.Name;

            RunResetHandler(localCodeExecutionContext);

#if DEBUG
            //Log("End");
#endif
        }

        private void ProcessResetResultWithItems(List<List<Var>> varList)
        {
#if DEBUG
            //Log("Begin");
#endif

            _isOn = false;

            if (_hasRuleInstancesList)
            {
                return;
            }

            foreach (var targetVarList in varList)
            {
                var localCodeExecutionContext = new LocalCodeExecutionContext();
                var localStorageSettings = RealStorageSettingsHelper.Create(_context, _storage);
                var storage = new LocalStorage(localStorageSettings);
                localCodeExecutionContext.Storage = storage;
                localCodeExecutionContext.Holder = _parent.Name;

                var varStorage = storage.VarStorage;

                foreach (var varItem in targetVarList)
                {
                    varStorage.Append(varItem);
                }

                RunResetHandler(localCodeExecutionContext);
            }

#if DEBUG
            //Log("End");
#endif
        }

        private void RunSetHandler(LocalCodeExecutionContext localCodeExecutionContext)
        {
#if DEBUG
            //Log($"_trigger = {_trigger}");
            //Log($"_trigger.CompiledFunctionBody = {_trigger.CompiledFunctionBody.ToDbgString()}");
            //Log($"_trigger.Priority = {_trigger.Priority}");
#endif

            var processInitialInfo = new ProcessInitialInfo();
            processInitialInfo.CompiledFunctionBody = _trigger.SetCompiledFunctionBody;
            processInitialInfo.LocalContext = localCodeExecutionContext;
            processInitialInfo.Metadata = _trigger;
            processInitialInfo.Instance = _parent;
            processInitialInfo.ExecutionCoordinator = _executionCoordinator;

#if DEBUG
            //Log($"processInitialInfo = {processInitialInfo}");
#endif

            var task = _context.CodeExecutor.ExecuteAsync(processInitialInfo);
        }

        private void RunResetHandler(LocalCodeExecutionContext localCodeExecutionContext)
        {
#if DEBUG
            //Log($"_trigger = {_trigger}");
            //Log($"_trigger.CompiledFunctionBody = {_trigger.CompiledFunctionBody.ToDbgString()}");
#endif

            var processInitialInfo = new ProcessInitialInfo();
            processInitialInfo.CompiledFunctionBody = _trigger.ResetCompiledFunctionBody;
            processInitialInfo.LocalContext = localCodeExecutionContext;
            processInitialInfo.Metadata = _trigger;
            processInitialInfo.Instance = _parent;
            processInitialInfo.ExecutionCoordinator = _executionCoordinator;

#if DEBUG
            //Log($"processInitialInfo = {processInitialInfo}");
#endif

            var task = _context.CodeExecutor.ExecuteAsync(processInitialInfo);
        }

        /// <inheritdoc/>
        protected override void OnDisposed()
        {
            _setConditionalTriggerObserver.OnChanged -= Observer_OnChanged;
            _setConditionalTriggerObserver.Dispose();

            if(_resetConditionalTriggerObserver != null)
            {
                _resetConditionalTriggerObserver.OnChanged -= Observer_OnChanged;
                _resetConditionalTriggerObserver.Dispose();
            }

            _setConditionalTriggerExecutor.Dispose();
            _resetConditionalTriggerExecutor?.Dispose();

            base.OnDisposed();
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return ToString(0u);
        }

        /// <inheritdoc/>
        public string ToString(uint n)
        {
            return this.GetDefaultToStringInformation(n);
        }

        /// <inheritdoc/>
        string IObjectToString.PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            return sb.ToString();
        }

        /// <inheritdoc/>
        public string ToShortString()
        {
            return ToShortString(0u);
        }

        /// <inheritdoc/>
        public string ToShortString(uint n)
        {
            return this.GetDefaultToShortStringInformation(n);
        }

        /// <inheritdoc/>
        string IObjectToShortString.PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            return sb.ToString();
        }

        /// <inheritdoc/>
        public string ToBriefString()
        {
            return ToBriefString(0u);
        }

        /// <inheritdoc/>
        public string ToBriefString(uint n)
        {
            return this.GetDefaultToBriefStringInformation(n);
        }

        /// <inheritdoc/>
        string IObjectToBriefString.PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            return sb.ToString();
        }
    }
}
