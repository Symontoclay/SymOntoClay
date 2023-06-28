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
        public LogicConditionalTriggerInstance(InlineTrigger trigger, BaseInstance parent, IEngineContext context, IStorage parentStorage, ILocalCodeExecutionContext parentCodeExecutionContext)
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

            var localCodeExecutionContext = new LocalCodeExecutionContext(parentCodeExecutionContext);
            var localStorageSettings = RealStorageSettingsHelper.Create(context, parentStorage);
            _storage = new LocalStorage(localStorageSettings);
            localCodeExecutionContext.Storage = _storage;

            localCodeExecutionContext.Holder = parent.Name;

            _localCodeExecutionContext = localCodeExecutionContext;

            _triggerConditionNodeObserverContext = new TriggerConditionNodeObserverContext(context, _storage, parent.Name);

            _setConditionalTriggerObserver = new LogicConditionalTriggerObserver(_triggerConditionNodeObserverContext, trigger.SetCondition, KindOfTriggerCondition.SetCondition);
            _setConditionalTriggerObserver.OnChanged += Observer_OnChanged;

            _ruleInstancesList = _trigger.RuleInstancesList;

            _hasRuleInstancesList = !_ruleInstancesList.IsNullOrEmpty();

            var setBindingVariables = trigger.SetBindingVariables;

            if(_hasRuleInstancesList)
            {
                setBindingVariables = new BindingVariables();
            }

            _setConditionalTriggerExecutor = new LogicConditionalTriggerExecutor(_triggerConditionNodeObserverContext,  trigger.SetCondition, KindOfTriggerCondition.SetCondition, trigger.SetBindingVariables, _localCodeExecutionContext);

            _hasResetHandler = trigger.ResetCompiledFunctionBody != null;

            if (_trigger.ResetCondition != null)
            {
                _hasResetConditions = true;

                _resetConditionalTriggerObserver = new LogicConditionalTriggerObserver(_triggerConditionNodeObserverContext, trigger.ResetCondition, KindOfTriggerCondition.ResetCondition);
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

                _resetConditionalTriggerExecutor = new LogicConditionalTriggerExecutor(_triggerConditionNodeObserverContext, trigger.ResetCondition, KindOfTriggerCondition.ResetCondition, resetBindingVariables, _localCodeExecutionContext);
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
        private readonly ILocalCodeExecutionContext _localCodeExecutionContext;
        private readonly LogicConditionalTriggerObserver _setConditionalTriggerObserver;
        private readonly LogicConditionalTriggerObserver _resetConditionalTriggerObserver;
        private readonly LogicConditionalTriggerExecutor _setConditionalTriggerExecutor;
        private readonly LogicConditionalTriggerExecutor _resetConditionalTriggerExecutor;

        private readonly object _lockObj = new object();
        private bool _isBusy;
        private bool _needRepeat;

        //private bool _isOn;

        private readonly bool _hasResetConditions;
        private readonly bool _hasResetHandler;

        /// <inheritdoc/>
        public IList<StrongIdentifierValue> NamesList => _namesList;

        /// <inheritdoc/>
        public bool IsOn => _triggerConditionNodeObserverContext.IsOn;

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
            Observer_OnChanged();
        }

        private void Observer_OnChanged()
        {
            Task.Run(() =>
            {
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

        }

        private void DoSearch()
        {
            var oldIsOn = _triggerConditionNodeObserverContext.IsOn;

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
            //Log($"_triggerConditionNodeObserverContext.IsOn = {_triggerConditionNodeObserverContext.IsOn}");
#endif

            if(_triggerConditionNodeObserverContext.IsOn)
            {
                if(!_triggerConditionNodeObserverContext.InitialDurationTime.HasValue)
                {
                    _triggerConditionNodeObserverContext.InitialDurationTime = _dateTimeProvider.CurrentTiks;
                }

                _triggerConditionNodeObserverContext.InitialEachTime = _dateTimeProvider.CurrentTiks;
            }
            else
            {
                if(_triggerConditionNodeObserverContext.InitialDurationTime.HasValue)
                {
                    _triggerConditionNodeObserverContext.InitialDurationTime = null;
                }

                if(!_triggerConditionNodeObserverContext.InitialEachTime.HasValue)
                {
                    _triggerConditionNodeObserverContext.InitialEachTime = _dateTimeProvider.CurrentTiks;
                }
            }

            if(_hasRuleInstancesList && oldIsOn != _triggerConditionNodeObserverContext.IsOn)
            {
                if(_triggerConditionNodeObserverContext.IsOn)
                {
                    _globalLogicalStorage.Append(_ruleInstancesList);
                }
                else
                {
                    _globalLogicalStorage.Remove(_ruleInstancesList);
                }
            }

            if(_hasNames && oldIsOn != _triggerConditionNodeObserverContext.IsOn)
            {
                Task.Run(() => {
                    OnChanged?.Invoke(_namesList);
                });
            }

        }

        private void DoSearchWithEqualConditions()
        {
            var setResult = _setConditionalTriggerExecutor.Run(out List<List<Var>> setVarList);

            if (setResult.IsSuccess)
            {
                if (setVarList.Any())
                {
                    ProcessSetResultWithItems(setVarList);
                }
                else
                {
                    ProcessSetResultWithNoItems(setResult.IsPeriodic);
                }
            }

            var resetResult = _resetConditionalTriggerExecutor.Run(out List<List<Var>> resetVarList);

            if (resetResult.IsSuccess)
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
                    _triggerConditionNodeObserverContext.IsOn = false;
                }
            }

        }

        private void DoSearchWithPriorSetCondition()
        {
            var setResult = _setConditionalTriggerExecutor.Run(out List<List<Var>> setVarList);

            if (setResult.IsSuccess)
            {
                if (setVarList.Any())
                {
                    ProcessSetResultWithItems(setVarList);
                }
                else
                {
                    ProcessSetResultWithNoItems(setResult.IsPeriodic);
                }
            }
            else
            {
                RunResetCondition();
            }

        }

        private void DoSearchWithPriorResetCondition()
        {
            var resetResult = _resetConditionalTriggerExecutor.Run(out List<List<Var>> resetVarList);

            if(resetResult.IsSuccess)
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
                if(!_triggerConditionNodeObserverContext.IsOn)
                {
                    var setResult = _setConditionalTriggerExecutor.Run(out List<List<Var>> setVarList);

                    if(setResult.IsSuccess)
                    {
                        if (setVarList.Any())
                        {
                            ProcessSetResultWithItems(setVarList);
                        }
                        else
                        {
                            ProcessSetResultWithNoItems(setResult.IsPeriodic);
                        }
                    }
                }
            }

        }

        private void RunResetCondition()
        {
            var resetResult = _resetConditionalTriggerExecutor.Run(out List<List<Var>> resetVarList);

            if (resetResult.IsSuccess)
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

        }

        private void DoSearchWithNoResetCondition()
        {
            var setResult = _setConditionalTriggerExecutor.Run(out List<List<Var>> setVarList);

#if DEBUG
            //Log($"setResult = {setResult}");
#endif

            if (setResult.IsSuccess)
            {
                if (setVarList.Any())
                {
                    ProcessSetResultWithItems(setVarList);
                }
                else
                {
                    ProcessSetResultWithNoItems(setResult.IsPeriodic);
                }
            }
            else
            {
                if (_hasResetHandler)
                {
                    ProcessResetResultWithNoItems();
                }

                _triggerConditionNodeObserverContext.IsOn = false;
            }

        }

        private void ProcessSetResultWithNoItems(bool isPeriodic)
        {
#if DEBUG
            //Log($"isPeriodic = {isPeriodic}");
            //Log($"_hasResetHandler = {_hasResetHandler}");
#endif

            if (_triggerConditionNodeObserverContext.IsOn)
            {
#if DEBUG
                //Log("_isOn return;");
#endif
                return;
            }

            if(!isPeriodic || _hasResetHandler)
            {
                _triggerConditionNodeObserverContext.IsOn = true;
            }
            else
            {
                _triggerConditionNodeObserverContext.InitialEachTime = _dateTimeProvider.CurrentTiks;
            }

            if(_hasRuleInstancesList)
            {
#if DEBUG
                //Log("_hasRuleInstancesList return;");
#endif

                return;
            }

            var localCodeExecutionContext = new LocalCodeExecutionContext(_localCodeExecutionContext);
            var localStorageSettings = RealStorageSettingsHelper.Create(_context, _storage);
            var storage = new LocalStorage(localStorageSettings);
            localCodeExecutionContext.Storage = storage;
            localCodeExecutionContext.Holder = _parent.Name;

            RunSetHandler(localCodeExecutionContext);

        }

        private void ProcessSetResultWithItems(List<List<Var>> varList)
        {
            _triggerConditionNodeObserverContext.IsOn = true;

            if (_hasRuleInstancesList)
            {
                return;
            }

            foreach (var targetVarList in varList)
            {
                var localCodeExecutionContext = new LocalCodeExecutionContext(_localCodeExecutionContext);
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

        }

        private void ProcessResetResultWithNoItems()
        {
            if(!_triggerConditionNodeObserverContext.IsOn)
            {
                return;
            }

            _triggerConditionNodeObserverContext.IsOn = false;

            if (_hasRuleInstancesList)
            {
                return;
            }

            var localCodeExecutionContext = new LocalCodeExecutionContext(_localCodeExecutionContext);
            var localStorageSettings = RealStorageSettingsHelper.Create(_context, _storage);
            var storage = new LocalStorage(localStorageSettings);
            localCodeExecutionContext.Storage = storage;
            localCodeExecutionContext.Holder = _parent.Name;

            RunResetHandler(localCodeExecutionContext);

        }

        private void ProcessResetResultWithItems(List<List<Var>> varList)
        {
            _triggerConditionNodeObserverContext.IsOn = false;

            if (_hasRuleInstancesList)
            {
                return;
            }

            foreach (var targetVarList in varList)
            {
                var localCodeExecutionContext = new LocalCodeExecutionContext(_localCodeExecutionContext);
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

        }

        private void RunSetHandler(ILocalCodeExecutionContext localCodeExecutionContext)
        {
            var processInitialInfo = new ProcessInitialInfo();
            processInitialInfo.CompiledFunctionBody = _trigger.SetCompiledFunctionBody;
            processInitialInfo.LocalContext = localCodeExecutionContext;
            processInitialInfo.Metadata = _trigger;
            processInitialInfo.Instance = _parent;
            processInitialInfo.ExecutionCoordinator = _executionCoordinator;

            var task = _context.CodeExecutor.ExecuteAsync(processInitialInfo);
        }

        private void RunResetHandler(ILocalCodeExecutionContext localCodeExecutionContext)
        {
            var processInitialInfo = new ProcessInitialInfo();
            processInitialInfo.CompiledFunctionBody = _trigger.ResetCompiledFunctionBody;
            processInitialInfo.LocalContext = localCodeExecutionContext;
            processInitialInfo.Metadata = _trigger;
            processInitialInfo.Instance = _parent;
            processInitialInfo.ExecutionCoordinator = _executionCoordinator;

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
