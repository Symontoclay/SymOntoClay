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
using NLog;
using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.Core.Internal.Instances.LogicConditionalTriggerExecutors;
using SymOntoClay.Core.Internal.Instances.LogicConditionalTriggerObservers;
using SymOntoClay.Core.Internal.Storage;
using SymOntoClay.Core.Internal.Threads;
using SymOntoClay.CoreHelper;
using SymOntoClay.CoreHelper.CollectionsHelpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.Monitor.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace SymOntoClay.Core.Internal.Instances
{
    public class LogicConditionalTriggerInstance : BaseComponent, INamedTriggerInstance, IObjectToString, IObjectToShortString, IObjectToBriefString
    {
        public LogicConditionalTriggerInstance(InlineTrigger trigger, BaseInstance parent, IEngineContext context, IStorage parentStorage, ILocalCodeExecutionContext parentCodeExecutionContext)
            : base(context.Logger)
        {
            Id = NameHelper.GetNewEntityNameString();

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



            _ruleInstancesList = _trigger.RuleInstancesList;

            _hasRuleInstancesList = !_ruleInstancesList.IsNullOrEmpty();

            var setBindingVariables = trigger.SetBindingVariables;

            if(_hasRuleInstancesList)
            {
                setBindingVariables = new BindingVariables();
            }

            _setConditionalTriggerExecutor = new LogicConditionalTriggerExecutor(_triggerConditionNodeObserverContext,  trigger.SetCondition, KindOfTriggerCondition.SetCondition, trigger.SetBindingVariables, _localCodeExecutionContext);

            _setConditionalTriggerObserver = new LogicConditionalTriggerObserver(_triggerConditionNodeObserverContext, trigger.SetCondition, KindOfTriggerCondition.SetCondition, _setConditionalTriggerExecutor.LocalCodeExecutionContext);
            _setConditionalTriggerObserver.OnChanged += Observer_OnChanged;

            _hasResetHandler = trigger.ResetCompiledFunctionBody != null;

            if (_trigger.ResetCondition != null)
            {
                _hasResetConditions = true;

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

                _resetConditionalTriggerObserver = new LogicConditionalTriggerObserver(_triggerConditionNodeObserverContext, trigger.ResetCondition, KindOfTriggerCondition.ResetCondition, _setConditionalTriggerExecutor.LocalCodeExecutionContext);
                _resetConditionalTriggerObserver.OnChanged += Observer_OnChanged;
            }
        }
        
        public string Id { get; }

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

        public void Init(IMonitorLogger logger)
        {
            Observer_OnChanged();
        }

        private void Observer_OnChanged()
        {
            Task.Run(() =>
            {
                try
                {
#if DEBUG
                    //Info("84B814BD-AFF4-487A-996C-8EF10370313E", $"_context.MonitorNode.MonitorFeatures.EnableDoTriggerSearch = {_context.MonitorNode.MonitorFeatures.EnableDoTriggerSearch}");
                    //Info("F9FC404C-A769-472E-9DD5-0DD0181C7067", $"_context.MonitorNode.MonitorFeatures.EnableEndDoTriggerSearch = {_context.MonitorNode.MonitorFeatures.EnableEndDoTriggerSearch}");
                    //Info("30E67EBB-82EE-4A40-B6B3-4575A155A105", $"_context.MonitorNode.MonitorFeatures.EnableSetConditionalTrigger = {_context.MonitorNode.MonitorFeatures.EnableSetConditionalTrigger}");
                    //Info("EA951532-84FD-436C-A933-ACE3DBED3AD6", $"_context.MonitorNode.MonitorFeatures.EnableResetConditionalTrigger = {_context.MonitorNode.MonitorFeatures.EnableResetConditionalTrigger}");
                    //Info("FED7C283-674F-4211-AF7E-DD643772DDC6", $"_context.MonitorNode.MonitorFeatures.EnableRunSetExprOfConditionalTrigger = {_context.MonitorNode.MonitorFeatures.EnableRunSetExprOfConditionalTrigger}");
                    //Info("42D93A77-E53A-41DE-9B2C-B4289F611FCA", $"_context.MonitorNode.MonitorFeatures.EnableEndRunSetExprOfConditionalTrigger = {_context.MonitorNode.MonitorFeatures.EnableEndRunSetExprOfConditionalTrigger}");
                    //Info("300C4325-C78D-47CB-9131-D072F274686F", $"_context.MonitorNode.MonitorFeatures.EnableRunResetExprOfConditionalTrigger = {_context.MonitorNode.MonitorFeatures.EnableRunResetExprOfConditionalTrigger}");
                    //Info("B52D9F68-D6A9-4078-8F1A-D375C16E3EC1", $"_context.MonitorNode.MonitorFeatures.EnableEndRunResetExprOfConditionalTrigger = {_context.MonitorNode.MonitorFeatures.EnableEndRunResetExprOfConditionalTrigger}");
                    //Info("091598FB-B589-4E81-B56A-57CD66A91134", $"_context.MonitorNode.MonitorFeatures.IsEnabledAnyConditionalTriggerFeature = {_context.MonitorNode.MonitorFeatures.IsEnabledAnyConditionalTriggerFeature}");
#endif

                    if(_context.MonitorNode.MonitorFeatures.IsEnabledAnyConditionalTriggerFeature)
                    {
                        var threadId = Guid.NewGuid().ToString("D");
                        var logger = _context.MonitorNode.CreateThreadLogger("3D06FC1B-69EB-49B4-B4A1-0184EE31A8D1", threadId: threadId, parentThreadId: Logger.Id);

                        NObserver_OnChanged(logger);
                    }
                    else
                    {
                        NObserver_OnChanged(Logger);
                    }
                }
                catch (Exception e)
                {
                    Error("7198C9A6-69B0-42F1-A1B2-44D2B42781CD", e);
                }
            });
        }

        private void NObserver_OnChanged(IMonitorLogger logger)
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

                DoSearch(logger);

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

                    DoSearch(logger);
                }
            }
            catch (Exception e)
            {
                logger.Error("7F5F198A-15CF-401F-9759-870B738DD315", e);
            }
        }

        private void DoSearch(IMonitorLogger logger)
        {
            var doTriggerSearchId = logger.DoTriggerSearch("8817B327-D587-4D5E-A5B6-25D7DFED1FDA", _parent.Name.ToHumanizedLabel(), _trigger.Holder.ToHumanizedLabel(), _trigger.ToLabel(logger));

            var oldIsOn = _triggerConditionNodeObserverContext.IsOn;

            if (_hasResetConditions)
            {
                switch(_trigger.DoubleConditionsStrategy)
                {
                    case DoubleConditionsStrategy.Equal:
                        DoSearchWithEqualConditions(logger, doTriggerSearchId);
                        break;

                    case DoubleConditionsStrategy.PriorSet:
                        DoSearchWithPriorSetCondition(logger, doTriggerSearchId);
                        break;

                    case DoubleConditionsStrategy.PriorReset:
                        DoSearchWithPriorResetCondition(logger, doTriggerSearchId);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(_trigger.DoubleConditionsStrategy), _trigger.DoubleConditionsStrategy, null);
                }
            }
            else
            {
                DoSearchWithNoResetCondition(logger, doTriggerSearchId);
            }

            if(_triggerConditionNodeObserverContext.IsOn)
            {
                if(!_triggerConditionNodeObserverContext.InitialResetTime.HasValue)
                {
                    _triggerConditionNodeObserverContext.InitialResetTime = _dateTimeProvider.CurrentTiks;
                }

                _triggerConditionNodeObserverContext.InitialSetTime = _dateTimeProvider.CurrentTiks;
            }
            else
            {
                if(_triggerConditionNodeObserverContext.InitialResetTime.HasValue)
                {
                    _triggerConditionNodeObserverContext.InitialResetTime = null;
                }

                if(!_triggerConditionNodeObserverContext.InitialSetTime.HasValue)
                {
                    _triggerConditionNodeObserverContext.InitialSetTime = _dateTimeProvider.CurrentTiks;
                }
            }

            if(_hasRuleInstancesList && oldIsOn != _triggerConditionNodeObserverContext.IsOn)
            {
                if(_triggerConditionNodeObserverContext.IsOn)
                {
                    _globalLogicalStorage.Append(Logger, _ruleInstancesList);
                }
                else
                {
                    _globalLogicalStorage.Remove(Logger, _ruleInstancesList);
                }
            }

            if(_hasNames && oldIsOn != _triggerConditionNodeObserverContext.IsOn)
            {
                Task.Run(() => {
                    OnChanged?.Invoke(_namesList);
                });
            }

            logger.EndDoTriggerSearch("4BB95054-441B-4372-9B33-7429D189BF5D", doTriggerSearchId);
        }

        private void SetIsOn(IMonitorLogger logger, string messagePointId, string doTriggerSearchId, bool value)
        {
            _triggerConditionNodeObserverContext.IsOn = value;

            if (value)
            {
                logger.SetConditionalTrigger(messagePointId, doTriggerSearchId);
            }
            else
            {
                logger.ResetConditionalTrigger(messagePointId, doTriggerSearchId);
            }            
        }

        private void DoSearchWithEqualConditions(IMonitorLogger logger, string doTriggerSearchId)
        {
            logger.RunSetExprOfConditionalTrigger("A086FCF0-5824-48A4-A7E9-EC01A47EC153", doTriggerSearchId, _trigger.SetCondition.ToLabel(logger));

            var setResult = _setConditionalTriggerExecutor.Run(out List<List<Var>> setVarList);

            logger.EndRunSetExprOfConditionalTrigger("BA00EF62-0857-41D7-AD90-50716AAA1342", doTriggerSearchId, _trigger.SetCondition.ToLabel(logger), setResult.IsSuccess, setResult.IsPeriodic, setVarList.Select(p => p.Select(x => x.ToLabel(logger)).ToList()).ToList());

            if (setResult.IsSuccess)
            {
                if (setVarList.Any())
                {
                    ProcessSetResultWithItems(logger, doTriggerSearchId, setVarList);
                }
                else
                {
                    ProcessSetResultWithNoItems(logger, doTriggerSearchId, setResult.IsPeriodic);
                }
            }

            logger.RunResetExprOfConditionalTrigger("C9EC20E0-ED95-4C49-A878-2F9FFE9B3891", doTriggerSearchId, _trigger.SetCondition.ToLabel(logger));

            var resetResult = _resetConditionalTriggerExecutor.Run(out List<List<Var>> resetVarList);

            logger.EndRunResetExprOfConditionalTrigger("235D11A4-0374-47C1-B7CD-CB6D7FC53BFF", doTriggerSearchId, _trigger.SetCondition.ToLabel(logger), resetResult.IsSuccess, resetResult.IsPeriodic, resetVarList.Select(p => p.Select(x => x.ToLabel(logger)).ToList()).ToList());

            if (resetResult.IsSuccess)
            {
                if (_hasResetHandler)
                {
                    if (resetVarList.Any())
                    {
                        ProcessResetResultWithItems(logger, doTriggerSearchId, resetVarList);
                    }
                    else
                    {
                        ProcessResetResultWithNoItems(logger, doTriggerSearchId);
                    }
                }
                else
                {
                    SetIsOn(logger, "259BEBE8-D672-40CD-8787-D512DB29FB55", doTriggerSearchId, false);
                }
            }
        }

        private void DoSearchWithPriorSetCondition(IMonitorLogger logger, string doTriggerSearchId)
        {
            logger.RunSetExprOfConditionalTrigger("DE4F238D-59EB-4463-A549-3A52BA64EEEB", doTriggerSearchId, _trigger.SetCondition.ToLabel(logger));

            var setResult = _setConditionalTriggerExecutor.Run(out List<List<Var>> setVarList);

            logger.EndRunSetExprOfConditionalTrigger("B8DEBAFF-87B5-4305-87B1-5E27CFB4E5A0", doTriggerSearchId, _trigger.SetCondition.ToLabel(logger), setResult.IsSuccess, setResult.IsPeriodic, setVarList.Select(p => p.Select(x => x.ToLabel(logger)).ToList()).ToList());

            if (setResult.IsSuccess)
            {
                if (setVarList.Any())
                {
                    ProcessSetResultWithItems(logger, doTriggerSearchId, setVarList);
                }
                else
                {
                    ProcessSetResultWithNoItems(logger, doTriggerSearchId, setResult.IsPeriodic);
                }
            }
            else
            {
                RunResetCondition(logger, doTriggerSearchId);
            }
        }

        private void DoSearchWithPriorResetCondition(IMonitorLogger logger, string doTriggerSearchId)
        {
            logger.RunResetExprOfConditionalTrigger("22CA222E-ABB4-42E0-A0E2-4A2DCE2AD5A4", doTriggerSearchId, _trigger.SetCondition.ToLabel(logger));

            var resetResult = _resetConditionalTriggerExecutor.Run(out List<List<Var>> resetVarList);

            logger.EndRunResetExprOfConditionalTrigger("36EED605-9DAC-4706-A880-4734648535C9", doTriggerSearchId, _trigger.SetCondition.ToLabel(logger), resetResult.IsSuccess, resetResult.IsPeriodic, resetVarList.Select(p => p.Select(x => x.ToLabel(logger)).ToList()).ToList());

            if (resetResult.IsSuccess)
            {
                if (_hasResetHandler)
                {
                    if (resetVarList.Any())
                    {
                        ProcessResetResultWithItems(logger, doTriggerSearchId, resetVarList);
                    }
                    else
                    {
                        ProcessResetResultWithNoItems(logger, doTriggerSearchId);
                    }
                }
            }
            else
            {
                if(!_triggerConditionNodeObserverContext.IsOn)
                {
                    logger.RunSetExprOfConditionalTrigger("1A43B211-4EA2-492C-A4F2-E4EA44C21E09", doTriggerSearchId, _trigger.SetCondition.ToLabel(logger));

                    var setResult = _setConditionalTriggerExecutor.Run(out List<List<Var>> setVarList);

                    logger.EndRunSetExprOfConditionalTrigger("BA80E9DA-39C0-4E5A-8527-BD97FA711919", doTriggerSearchId, _trigger.SetCondition.ToLabel(logger), setResult.IsSuccess, setResult.IsPeriodic, setVarList.Select(p => p.Select(x => x.ToLabel(logger)).ToList()).ToList());

                    if (setResult.IsSuccess)
                    {
                        if (setVarList.Any())
                        {
                            ProcessSetResultWithItems(logger, doTriggerSearchId, setVarList);
                        }
                        else
                        {
                            ProcessSetResultWithNoItems(logger, doTriggerSearchId, setResult.IsPeriodic);
                        }
                    }
                }
            }
        }

        private void RunResetCondition(IMonitorLogger logger, string doTriggerSearchId)
        {
            logger.RunResetExprOfConditionalTrigger("6AFE26AA-AC1B-4286-B8B9-2EEB53A9B973", doTriggerSearchId, _trigger.SetCondition.ToLabel(logger));

            var resetResult = _resetConditionalTriggerExecutor.Run(out List<List<Var>> resetVarList);

            logger.EndRunResetExprOfConditionalTrigger("92237553-81D4-4123-B98A-F350D9087024", doTriggerSearchId, _trigger.SetCondition.ToLabel(logger), resetResult.IsSuccess, resetResult.IsPeriodic, resetVarList.Select(p => p.Select(x => x.ToLabel(logger)).ToList()).ToList());

            if (resetResult.IsSuccess)
            {
                if (_hasResetHandler)
                {
                    if (resetVarList.Any())
                    {
                        ProcessResetResultWithItems(logger, doTriggerSearchId, resetVarList);
                    }
                    else
                    {
                        ProcessResetResultWithNoItems(logger, doTriggerSearchId);
                    }
                }
            }
        }

        private void DoSearchWithNoResetCondition(IMonitorLogger logger, string doTriggerSearchId)
        {
            logger.RunSetExprOfConditionalTrigger("E5C33894-4D43-41C5-86AF-FB31ABCF9699", doTriggerSearchId, _trigger.SetCondition.ToLabel(logger));

            var setResult = _setConditionalTriggerExecutor.Run(out List<List<Var>> setVarList);

            logger.EndRunSetExprOfConditionalTrigger("6BEB4E66-8CD3-4A08-923E-1D0B2CB93A98", doTriggerSearchId, _trigger.SetCondition.ToLabel(logger), setResult.IsSuccess, setResult.IsPeriodic, setVarList.Select(p => p.Select(x => x.ToLabel(logger)).ToList()).ToList());

            if (setResult.IsSuccess)
            {
                if (setVarList.Any())
                {
                    ProcessSetResultWithItems(logger, doTriggerSearchId, setVarList);
                }
                else
                {
                    ProcessSetResultWithNoItems(logger, doTriggerSearchId, setResult.IsPeriodic);
                }
            }
            else
            {
                if (_hasResetHandler)
                {
                    ProcessResetResultWithNoItems(logger, doTriggerSearchId);
                }

                SetIsOn(logger, "9DBF33D0-ABAE-43DF-A0ED-DB2D153791F0", doTriggerSearchId, false);
            }
        }

        private void ProcessSetResultWithNoItems(IMonitorLogger logger, string doTriggerSearchId, bool isPeriodic)
        {
            if (_triggerConditionNodeObserverContext.IsOn)
            {
                return;
            }

            if(!isPeriodic || _hasResetHandler)
            {
                SetIsOn(logger, "16A8DE52-E9A1-4CE1-BE8F-DA4FDC526977", doTriggerSearchId, true);
            }
            else
            {
                _triggerConditionNodeObserverContext.InitialSetTime = _dateTimeProvider.CurrentTiks;
            }

            if(_hasRuleInstancesList)
            {
                return;
            }

            var localCodeExecutionContext = new LocalCodeExecutionContext(_localCodeExecutionContext);
            var localStorageSettings = RealStorageSettingsHelper.Create(_context, _storage);
            var storage = new LocalStorage(localStorageSettings);
            localCodeExecutionContext.Storage = storage;
            localCodeExecutionContext.Holder = _parent.Name;

            RunSetHandler(logger, doTriggerSearchId, localCodeExecutionContext);
        }

        private void ProcessSetResultWithItems(IMonitorLogger logger, string doTriggerSearchId, List<List<Var>> varList)
        {
            SetIsOn(logger, "C91DAF33-135D-44C9-B6DE-155F59D1DB80", doTriggerSearchId, true);

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
                    varStorage.Append(Logger, varItem);
                }

                RunSetHandler(logger, doTriggerSearchId, localCodeExecutionContext);
            }
        }

        private void ProcessResetResultWithNoItems(IMonitorLogger logger, string doTriggerSearchId)
        {
            if(!_triggerConditionNodeObserverContext.IsOn)
            {
                return;
            }

            SetIsOn(logger, "C0FBDBCB-0F4E-4BBB-9973-E43D89D9C1CE", doTriggerSearchId, false);

            if (_hasRuleInstancesList)
            {
                return;
            }

            var localCodeExecutionContext = new LocalCodeExecutionContext(_localCodeExecutionContext);
            var localStorageSettings = RealStorageSettingsHelper.Create(_context, _storage);
            var storage = new LocalStorage(localStorageSettings);
            localCodeExecutionContext.Storage = storage;
            localCodeExecutionContext.Holder = _parent.Name;

            RunResetHandler(logger, doTriggerSearchId, localCodeExecutionContext);
        }

        private void ProcessResetResultWithItems(IMonitorLogger logger, string doTriggerSearchId, List<List<Var>> varList)
        {
            SetIsOn(logger, "ABF47267-915E-467E-93D1-E44A548A3D1D", doTriggerSearchId, false);

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
                    varStorage.Append(Logger, varItem);
                }

                RunResetHandler(logger, doTriggerSearchId, localCodeExecutionContext);
            }
        }

        private void RunSetHandler(IMonitorLogger logger, string doTriggerSearchId, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            var processInitialInfo = new ProcessInitialInfo();
            processInitialInfo.CompiledFunctionBody = _trigger.SetCompiledFunctionBody;
            processInitialInfo.LocalContext = localCodeExecutionContext;
            processInitialInfo.Metadata = _trigger;
            processInitialInfo.Instance = _parent;
            processInitialInfo.ExecutionCoordinator = _executionCoordinator;

            var task = _context.CodeExecutor.ExecuteAsync(Logger, processInitialInfo);
        }

        private void RunResetHandler(IMonitorLogger logger, string doTriggerSearchId, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            var processInitialInfo = new ProcessInitialInfo();
            processInitialInfo.CompiledFunctionBody = _trigger.ResetCompiledFunctionBody;
            processInitialInfo.LocalContext = localCodeExecutionContext;
            processInitialInfo.Metadata = _trigger;
            processInitialInfo.Instance = _parent;
            processInitialInfo.ExecutionCoordinator = _executionCoordinator;

            var task = _context.CodeExecutor.ExecuteAsync(Logger, processInitialInfo);
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
            sb.Append($"{spaces}{nameof(Id)} = {Id}");
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
            sb.Append($"{spaces}{nameof(Id)} = {Id}");
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
            sb.Append($"{spaces}{nameof(Id)} = {Id}");
            return sb.ToString();
        }
    }
}
