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

using SymOntoClay.ActiveObject.Functors;
using SymOntoClay.ActiveObject.Threads;
using SymOntoClay.Common;
using SymOntoClay.Common.CollectionsHelpers;
using SymOntoClay.Common.DebugHelpers;
using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.EventsInterfaces;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Monitor.Common.Models;
using SymOntoClay.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace SymOntoClay.Core.Internal.Storage.LogicalStoraging
{
    public class LogicalStorage: BaseSpecificStorage, ILogicalStorage,
        IOnAddingFactHandler, IOnAddParentStorageRealStorageContextHandler, IOnRemoveParentStorageRealStorageContextHandler, IOnChangedLogicalStorageHandler, IOnChangedWithKeysLogicalStorageHandler,
        ILogicalStorageSerializedEventsHandler
    {
        private const int DEFAULT_INITIAL_TIME = 20;

        public LogicalStorage(KindOfStorage kind, RealStorageContext realStorageContext)
            : base(kind, realStorageContext)
        {
            _activeObjectContext = _mainStorageContext.ActiveObjectContext;
            _threadPool = _mainStorageContext.AsyncEventsThreadPool;
            _serializationAnchor = new SerializationAnchor();

            _dateTimeProvider = _mainStorageContext.DateTimeProvider;

#if DEBUG
            //Info("16C31D0E-A3FD-49EA-B723-7A75258CA75E", $"Kind = {Kind}");
            //Info("1D7C71CE-31F9-4C61-B8B8-7122D64265A7", $"_dateTimeProvider == null = {_dateTimeProvider == null}");
#endif

            _fuzzyLogicResolver = _mainStorageContext.DataResolversFactory.GetFuzzyLogicResolver();

            var logger = _mainStorageContext.Logger;

            _parentLogicalStoragesList = realStorageContext.Parents.Select(p => p.LogicalStorage).ToList();

            _ruleInstancesList = new List<RuleInstance>();
            _factsList = new List<RuleInstance>();
            _ruleInstancesDict = new Dictionary<StrongIdentifierValue, RuleInstance>();
            _ruleInstancesDictByHashCode = new Dictionary<ulong, RuleInstance>();
            _ruleInstancesDictById = new Dictionary<string, RuleInstance>();
            _lifeTimeCycleById = new Dictionary<string, int>();
            _commonPersistIndexedLogicalData = new CommonPersistIndexedLogicalData(logger);
            _enableAddingRemovingFactLoggingInStorages = logger.EnableAddingRemovingFactLoggingInStorages;

            foreach (var parentStorage in _parentLogicalStoragesList)
            {
                parentStorage.AddOnChangedWithKeysHandler(this);
                parentStorage.AddOnChangedHandler(this);
                parentStorage.AddOnAddingFactHandler(this);
            }

            realStorageContext.AddOnAddParentStorageHandler(this);
            realStorageContext.AddOnRemoveParentStorageHandler(this);

            _enableOnAddingFactEvent = realStorageContext.EnableOnAddingFactEvent;

            if(_enableOnAddingFactEvent)
            {
                var localCodeExecutionContext = new LocalCodeExecutionContext(realStorageContext.ParentCodeExecutionContext);
                localCodeExecutionContext.Storage = _mainStorageContext.Storage.GlobalStorage;
                localCodeExecutionContext.Holder = NameHelper.CreateName(_mainStorageContext.Id);

                _localCodeExecutionContext = localCodeExecutionContext;
            }

            var kindOfGC = realStorageContext.KindOfGC;

            switch(kindOfGC)
            {
                case KindOfGC.None:
                    break;

                case KindOfGC.ByTimeOut:
                    InitGCByTimeOut();
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfGC), kindOfGC, null);
            }
        }

        private readonly object _lockObj = new object();

        private List<RuleInstance> _ruleInstancesList;
        private List<RuleInstance> _factsList;
        private Dictionary<StrongIdentifierValue, RuleInstance> _ruleInstancesDict;
        private Dictionary<ulong, RuleInstance> _ruleInstancesDictByHashCode;
        private Dictionary<string, RuleInstance> _ruleInstancesDictById;
        private Dictionary<string, int> _lifeTimeCycleById;
        private readonly bool _enableAddingRemovingFactLoggingInStorages;

        private readonly CommonPersistIndexedLogicalData _commonPersistIndexedLogicalData;
        private List<ILogicalStorage> _parentLogicalStoragesList = new List<ILogicalStorage>();

        private IActiveObjectContext _activeObjectContext;
        private ICustomThreadPool _threadPool;
        private SerializationAnchor _serializationAnchor;

        private AsyncActivePeriodicObject _activeObject;

        private readonly bool _enableOnAddingFactEvent;

        private readonly FuzzyLogicResolver _fuzzyLogicResolver;
        private readonly ILocalCodeExecutionContext _localCodeExecutionContext;

        private IDateTimeProvider _dateTimeProvider;

        private void InitGCByTimeOut()
        {
            _activeObject = new AsyncActivePeriodicObject(_mainStorageContext.ActiveObjectContext, _mainStorageContext.AsyncEventsThreadPool, Logger);
            _activeObject.PeriodicMethod = GCByTimeOutCommandLoop;
            _activeObject.Start();
        }

        /// <inheritdoc/>
        public void Append(IMonitorLogger logger, RuleInstance ruleInstance)
        {
            Append(logger, ruleInstance, true);
        }

        /// <inheritdoc/>
        public void Append(IMonitorLogger logger, RuleInstance ruleInstance, bool isPrimary)
        {
            lock (_lockObj)
            {
                var usedKeysList = NAppendAndReturnUsedKeysList(logger, ruleInstance, isPrimary);

                EmitOnChanged(logger, usedKeysList);
            }
        }

        /// <inheritdoc/>
        public void Append(IMonitorLogger logger, IList<RuleInstance> ruleInstancesList)
        {
            lock (_lockObj)
            {
                var usedKeysList = new List<StrongIdentifierValue>();

                foreach(var item in ruleInstancesList)
                {
                    usedKeysList.AddRange(NAppendAndReturnUsedKeysList(logger, item, true));
                }

                usedKeysList = usedKeysList.Distinct().ToList();

                EmitOnChanged(logger, usedKeysList);
            }
        }

        private IList<StrongIdentifierValue> NAppendAndReturnUsedKeysList(IMonitorLogger logger, RuleInstance ruleInstance, bool isPrimary)
        {
            if (NAppend(logger, ruleInstance, isPrimary))
            {
                var annotationsList = ruleInstance.GetAllAnnotations();

                var annotationRuleInstancesList = annotationsList.Where(p => !p.Facts.IsNullOrEmpty()).SelectMany(p => p.Facts);

                foreach (var annotationRuleInstance in annotationRuleInstancesList)
                {
                    NAppend(logger, annotationRuleInstance, false);
                }

                return ruleInstance.Normalized.UsedKeysList.Concat(annotationRuleInstancesList.SelectMany(p => p.UsedKeysList)).Distinct().ToList();
            }

            return new List<StrongIdentifierValue>();
        }

        private bool NAppend(IMonitorLogger logger, RuleInstance ruleInstance, bool isPrimary)
        {
#if DEBUG
            //Info("B8355FCB-99E5-4020-8962-CA92E038FF8E", $"ruleInstance = {ruleInstance?.ToHumanizedString()}");
#endif

            ruleInstance = ProcessOnAddingFactHandlers(logger, ruleInstance, isPrimary);

            if(ruleInstance == null)
            {
                return false;
            }

            ruleInstance.CheckDirty();

#if DEBUG
            //Info("D912C45C-3C03-4E3D-A99A-30E6FAEE5C4C", $"ruleInstance (after) = {ruleInstance?.ToHumanizedString()}");
#endif

            if (ruleInstance.TypeOfAccess != TypeOfAccess.Local)
            {
                AnnotatedItemHelper.CheckAndFillUpHolder(logger, ruleInstance, _realStorageContext.MainStorageContext.CommonNamesStorage);
            }

            if (_ruleInstancesList.Contains(ruleInstance))
            {
                RefreshLifeTime(logger, ruleInstance);

                return true;
            }

            var ruleInstanceId = ruleInstance.Name.NameValue;

            if (_ruleInstancesDictById.ContainsKey(ruleInstanceId))
            {
                RefreshLifeTime(logger, ruleInstanceId);

                return true;
            }

            var ruleInstanceName = ruleInstance.Name;

            if (_ruleInstancesDict.ContainsKey(ruleInstanceName))
            {
                RefreshLifeTime(logger, ruleInstanceName);

                return true;
            }

            var longHashCode = ruleInstance.GetLongHashCode();

            if(_ruleInstancesDictByHashCode.ContainsKey(longHashCode))
            {
                RefreshLifeTime(logger, longHashCode);

                return true;
            }

            ruleInstance.AddLogicalStorage(this);

#if DEBUG
            //Info("5CF944E7-E757-4FF1-8A01-78CB32569EC7", $"ruleInstance.GetLongHashCode() = {ruleInstance.GetLongHashCode()}");
            //Info("225D346C-31BC-4C82-87A2-370286D40844", $"ruleInstance = {ruleInstance}");
#endif

            if (!ruleInstance.TimeStamp.HasValue && _dateTimeProvider != null)
            {
                ruleInstance.TimeStamp = _dateTimeProvider.CurrentTicks;
            }

#if DEBUG           
            //Info("400CE6A1-5288-4B61-935C-F6380840F178", $"ruleInstance = {ruleInstance}");
#endif

            if(_enableAddingRemovingFactLoggingInStorages)
            {
                logger.AddFactToLogicalStorage("6D10091F-6FD6-4150-88DB-854F7CE890C8", ruleInstance.ToLabel(logger), this.ToLabel(logger));
            }

            _ruleInstancesList.Add(ruleInstance);

            if(ruleInstance.KindOfRuleInstance == KindOfRuleInstance.Fact)
            {
                _factsList.Add(ruleInstance);
            }

            _ruleInstancesDict[ruleInstanceName] = ruleInstance;
            _ruleInstancesDictByHashCode[longHashCode] = ruleInstance;
            _ruleInstancesDictById[ruleInstanceId] = ruleInstance;
            _lifeTimeCycleById[ruleInstanceId] = DEFAULT_INITIAL_TIME;

            _commonPersistIndexedLogicalData.NSetIndexedRuleInstanceToIndexData(logger, ruleInstance.Normalized, false);

            if(isPrimary && _kind != KindOfStorage.PublicFacts && _kind != KindOfStorage.PerceptedFacts)
            {
                var inheritanceRelationsList = ruleInstance.GetInheritanceRelations(logger);

                if (inheritanceRelationsList.Any())
                {
                    var inheritanceStorage = _realStorageContext.InheritanceStorage;

                    foreach (var inheritanceRelation in inheritanceRelationsList)
                    {
                        var inheritanceItem = new InheritanceItem();

                        if(inheritanceRelation.Name == NameHelper.CreateName("is"))
                        {
                            var paramsCount = inheritanceRelation.ParamsList.Count;

                            switch(paramsCount)
                            {
                                case 2:
                                    inheritanceItem.SuperName = inheritanceRelation.ParamsList[1].Name;
                                    inheritanceItem.SubName = inheritanceRelation.ParamsList[0].Name;
                                    inheritanceItem.Rank = new LogicalValue(1);
                                    break;

                                case 3:
                                    inheritanceItem.SuperName = inheritanceRelation.ParamsList[1].Name;
                                    inheritanceItem.SubName = inheritanceRelation.ParamsList[0].Name;
                                    var thirdParameter = inheritanceRelation.ParamsList[2];
                                    var kindOfThirdParameter = thirdParameter.Kind;
                                    switch (kindOfThirdParameter)
                                    {
                                        case KindOfLogicalQueryNode.Concept:
                                            inheritanceItem.Rank = thirdParameter.Name;
                                            break;

                                        case KindOfLogicalQueryNode.FuzzyLogicNonNumericSequence:
                                            inheritanceItem.Rank = thirdParameter.FuzzyLogicNonNumericSequenceValue;
                                            break;

                                        case KindOfLogicalQueryNode.Value:
                                            inheritanceItem.Rank = thirdParameter.Value;
                                            break;

                                        default:
                                            throw new ArgumentOutOfRangeException(nameof(kindOfThirdParameter), kindOfThirdParameter, null);
                                    }
                                    break;

                                default:
                                    throw new ArgumentOutOfRangeException(nameof(paramsCount), paramsCount, null);
                            }                            
                        }
                        else
                        {
                            inheritanceItem.SuperName = inheritanceRelation.Name;
                            inheritanceItem.SubName = inheritanceRelation.ParamsList.Single().Name;
                            inheritanceItem.Rank = new LogicalValue(1);
                        }
                        
                        inheritanceItem.KeysOfPrimaryRecords.Add(ruleInstanceName);
                        inheritanceStorage.SetInheritance(logger, inheritanceItem, false);
                    }
                }
            }

            return true;
        }

        private RuleInstance ProcessOnAddingFactHandlers(IMonitorLogger logger, RuleInstance ruleInstance, bool isPrimary)
        {
            if (_enableOnAddingFactEvent && _onAddingFactHandlers.Count > 0)
            {
                if (isPrimary && ruleInstance.KindOfRuleInstance == KindOfRuleInstance.Fact)
                {
                    var approvingRez = AddingFactHelper.CallEvent(logger, _onAddingFactHandlers, ruleInstance, _fuzzyLogicResolver, _localCodeExecutionContext);

                    if (_enableAddingRemovingFactLoggingInStorages)
                    {
                        logger.AddFactOrRuleTriggerResult("E7837A32-70CC-4F89-9206-1DC386542546", ruleInstance.ToLabel(logger), this.ToLabel(logger), approvingRez?.ToLabel(logger));
                    }

                    if (approvingRez != null)
                    {
                        var kindOfResult = approvingRez.KindOfResult;

                        switch (kindOfResult)
                        {
                            case KindOfAddFactOrRuleResult.Reject:
                                return null;

                            case KindOfAddFactOrRuleResult.Accept:
                                return approvingRez.ChangedRuleInstance;

                            default:
                                throw new ArgumentOutOfRangeException(nameof(kindOfResult), kindOfResult, null);
                        }
                    }
                }
            }

            return ruleInstance;
        }

        /// <inheritdoc/>
        public void Remove(IMonitorLogger logger, RuleInstance ruleInstance)
        {
            lock (_lockObj)
            {
                NRemove(logger, ruleInstance);
            }
        }

        /// <inheritdoc/>
        public void Remove(IMonitorLogger logger, IList<RuleInstance> ruleInstancesList)
        {
            lock (_lockObj)
            {
                var usedKeysList = new List<StrongIdentifierValue>();

                foreach(var item in ruleInstancesList)
                {
                    var tmpUsedKeysList = NRemoveAndReturnUsedKeysList(logger, item);

                    if(tmpUsedKeysList != null)
                    {
                        usedKeysList.AddRange(tmpUsedKeysList);
                    }
                }

                usedKeysList = usedKeysList.Distinct().ToList();

                EmitOnChanged(logger, usedKeysList);
            }
        }

        /// <inheritdoc/>
        public void RemoveById(IMonitorLogger logger, string id)
        {
            lock (_lockObj)
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    return;
                }

                NRemoveById(logger, id);
            }
        }

        private void NRemoveById(IMonitorLogger logger, string id)
        {
            if (_ruleInstancesDictById.ContainsKey(id))
            {
                NRemove(logger, _ruleInstancesDictById[id]);
            }
        }

        private void NRemove(IMonitorLogger logger, RuleInstance ruleInstance)
        {
            var usedKeysList = NRemoveAndReturnUsedKeysList(logger, ruleInstance);

            if (usedKeysList.IsNullOrEmpty())
            {
                EmitOnChanged(logger);
            }
            else 
            { 
                EmitOnChanged(logger, usedKeysList);
            }
        }

        private List<StrongIdentifierValue> NRemoveAndReturnUsedKeysList(IMonitorLogger logger, RuleInstance ruleInstance)
        {
            if (!_ruleInstancesList.Contains(ruleInstance))
            {
                return null;
            }

            ruleInstance.RemoveFromLogicalStorage(this);

            _ruleInstancesList.Remove(ruleInstance);

            _factsList.Remove(ruleInstance);

            var ruleInstanceName = ruleInstance.Name;

            _ruleInstancesDict.Remove(ruleInstanceName);

            var longHashCode = ruleInstance.GetLongHashCode();

            _ruleInstancesDictByHashCode.Remove(longHashCode);

            var ruleInstanceId = ruleInstance.Name.NameValue;

            _ruleInstancesDictById.Remove(ruleInstanceId);

            _lifeTimeCycleById.Remove(ruleInstanceId);

            _commonPersistIndexedLogicalData.NRemoveIndexedRuleInstanceFromIndexData(logger, ruleInstance.Normalized);

            if(_enableAddingRemovingFactLoggingInStorages)
            {
                logger.RemoveFactFromLogicalStorage("62B03746-90F3-4BE7-9E69-71BBC71583E8", ruleInstance.ToLabel(logger), ToLabel(logger));
            }

            return ruleInstance.UsedKeysList;
        }

        protected void EmitOnChanged(IMonitorLogger logger, IList<StrongIdentifierValue> usedKeysList)
        {
            EmitOnChanged(logger);

            EmitOnChangedWithKeysHandlers(usedKeysList);
        }

        protected void EmitOnChanged(IMonitorLogger logger)
        {
            EmitOnChangedHandlers();
        }

        void IOnChangedWithKeysLogicalStorageHandler.Invoke(IList<StrongIdentifierValue> value)
        {
            LogicalStorage_OnChangedWithKeys(value);
        }

        private void LogicalStorage_OnChangedWithKeys(IList<StrongIdentifierValue> changedKeysList)
        {
            EmitOnChanged(Logger, changedKeysList);
        }

        void IOnChangedLogicalStorageHandler.Invoke()
        {
            LogicalStorage_OnChanged();
        }

        private void LogicalStorage_OnChanged()
        {
            EmitOnChanged(Logger);
        }

        void ILogicalStorage.AddOnChangedHandler(IOnChangedLogicalStorageHandler handler)
        {
            lock (_onChangedHandlersLockObj)
            {
                if (_onChangedHandlers.Contains(handler))
                {
                    return;
                }

                _onChangedHandlers.Add(handler);
            }
        }

        void ILogicalStorage.RemoveOnChangedHandler(IOnChangedLogicalStorageHandler handler)
        {
            lock (_onChangedHandlersLockObj)
            {
                if (_onChangedHandlers.Contains(handler))
                {
                    _onChangedHandlers.Remove(handler);
                }
            }
        }

        private void EmitOnChangedHandlers()
        {
            lock (_onChangedHandlersLockObj)
            {
                foreach (var handler in _onChangedHandlers)
                {
                    handler.Invoke();
                }
            }
        }

        private object _onChangedHandlersLockObj = new object();
        private List<IOnChangedLogicalStorageHandler> _onChangedHandlers = new List<IOnChangedLogicalStorageHandler>();

        void ILogicalStorage.AddOnChangedWithKeysHandler(IOnChangedWithKeysLogicalStorageHandler handler)
        {
            lock (_onChangedWithKeysHandlersLockObj)
            {
                if (_onChangedWithKeysHandlers.Contains(handler))
                {
                    return;
                }

                _onChangedWithKeysHandlers.Add(handler);
            }
        }

        void ILogicalStorage.RemoveOnChangedWithKeysHandler(IOnChangedWithKeysLogicalStorageHandler handler)
        {
            lock (_onChangedWithKeysHandlersLockObj)
            {
                if (_onChangedWithKeysHandlers.Contains(handler))
                {
                    _onChangedWithKeysHandlers.Remove(handler);
                }
            }
        }

        private void EmitOnChangedWithKeysHandlers(IList<StrongIdentifierValue> value)
        {
            lock (_onChangedWithKeysHandlersLockObj)
            {
                foreach (var handler in _onChangedWithKeysHandlers)
                {
                    handler.Invoke(value);
                }
            }
        }

        private object _onChangedWithKeysHandlersLockObj = new object();
        private List<IOnChangedWithKeysLogicalStorageHandler> _onChangedWithKeysHandlers = new List<IOnChangedWithKeysLogicalStorageHandler>();

        private object _onAddingFactHandlerLockObj = new object();
        private List<IOnAddingFactHandler> _onAddingFactHandlers = new List<IOnAddingFactHandler>();

        /// <inheritdoc/>
        public void AddOnAddingFactHandler(IOnAddingFactHandler handler)
        {
            lock (_onAddingFactHandlerLockObj)
            {
                if (_onAddingFactHandlers.Contains(handler))
                {
                    return;
                }

                _onAddingFactHandlers.Add(handler);
            }
        }

        /// <inheritdoc/>
        public void RemoveOnAddingFactHandler(IOnAddingFactHandler handler)
        {
            lock (_onAddingFactHandlerLockObj)
            {
                if (_onAddingFactHandlers.Contains(handler))
                {
                    _onAddingFactHandlers.Remove(handler);
                }
            }
        }

        IAddFactOrRuleResult IOnAddingFactHandler.OnAddingFact(IMonitorLogger logger, RuleInstance fact)
        {
            return LogicalStorage_OnAddingFact(logger, fact);
        }

        private IAddFactOrRuleResult LogicalStorage_OnAddingFact(IMonitorLogger logger, RuleInstance ruleInstance)
        {
            if(_onAddingFactHandlers.Count == 0)
            {
                return null;
            }

            return AddingFactHelper.CallEvent(logger, _onAddingFactHandlers, ruleInstance, _fuzzyLogicResolver, _localCodeExecutionContext);
        }

        void IOnAddParentStorageRealStorageContextHandler.Invoke(IStorage storage)
        {
            RealStorageContext_OnAddParentStorage(storage);
        }

        private void RealStorageContext_OnAddParentStorage(IStorage storage)
        {
            LoggedFunctorWithoutResult<ILogicalStorageSerializedEventsHandler, IStorage>.Run(Logger, "B0A51B85-F56B-4A35-A310-6616A97899DB", this, storage,
                (IMonitorLogger loggerValue, ILogicalStorageSerializedEventsHandler instanceValue, IStorage storageValue) => {
                    instanceValue.NRealStorageContext_OnAddParentStorage(storageValue);
                },
                _activeObjectContext, _threadPool, _serializationAnchor);
        }

        void ILogicalStorageSerializedEventsHandler.NRealStorageContext_OnAddParentStorage(IStorage storage)
        {
            var logicalStorage = storage.LogicalStorage;
            logicalStorage.AddOnChangedWithKeysHandler(this);
            logicalStorage.AddOnChangedHandler(this);
            logicalStorage.AddOnAddingFactHandler(this);

            _parentLogicalStoragesList.Add(logicalStorage);
        }

        void IOnRemoveParentStorageRealStorageContextHandler.Invoke(IStorage storage)
        {
            RealStorageContext_OnRemoveParentStorage(storage);
        }

        private void RealStorageContext_OnRemoveParentStorage(IStorage storage)
        {
            LoggedFunctorWithoutResult<ILogicalStorageSerializedEventsHandler, IStorage>.Run(Logger, "1A999A30-68D4-4166-99FE-A02569B22854", this, storage,
                (IMonitorLogger loggerValue, ILogicalStorageSerializedEventsHandler instanceValue, IStorage storageValue) => {
                    instanceValue.NRealStorageContext_OnRemoveParentStorage(storageValue);
                },
                _activeObjectContext, _threadPool, _serializationAnchor);
        }

        void ILogicalStorageSerializedEventsHandler.NRealStorageContext_OnRemoveParentStorage(IStorage storage)
        {
            var logicalStorage = storage.LogicalStorage;
            logicalStorage.RemoveOnChangedWithKeysHandler(this);
            logicalStorage.RemoveOnChangedHandler(this);
            logicalStorage.RemoveOnAddingFactHandler(this);

            _parentLogicalStoragesList.Remove(logicalStorage);
        }

        private static List<LogicalQueryNode> _emptyLogicalQueryNodesList = new List<LogicalQueryNode>();

        /// <inheritdoc/>
        public IList<LogicalQueryNode> GetAllRelations(IMonitorLogger logger, ILogicalSearchStorageContext logicalSearchStorageContext, LogicalSearchExplainNode parentExplainNode, LogicalSearchExplainNode rootParentExplainNode)
        {
            lock (_lockObj)
            {
                if (_realStorageContext.Disabled)
                {
                    return _emptyLogicalQueryNodesList;
                }

                LogicalSearchExplainNode currentExplainNode = null;

                if (parentExplainNode != null)
                {
                    currentExplainNode = new LogicalSearchExplainNode(rootParentExplainNode)
                    {
                        Kind = KindOfLogicalSearchExplainNode.LogicalStorage,
                        LogicalStorage = this
                    };
                }

                var source = _commonPersistIndexedLogicalData.GetAllRelations(logger);

                if (logicalSearchStorageContext == null || source.IsNullOrEmpty())
                {
                    if (parentExplainNode != null)
                    {
                        LogicalSearchExplainNode.LinkNodes(parentExplainNode, currentExplainNode);
                    }

                    return source;
                }

                LogicalSearchExplainNode filteringExplainNode = null;

                if (parentExplainNode != null)
                {
                    filteringExplainNode = new LogicalSearchExplainNode(rootParentExplainNode)
                    {
                        Kind = KindOfLogicalSearchExplainNode.LogicalStorageFilter
                    };

                    LogicalSearchExplainNode.LinkNodes(parentExplainNode, filteringExplainNode);

                    var intermediateResultExplainNode = new LogicalSearchExplainNode(rootParentExplainNode)
                    {
                        Kind = KindOfLogicalSearchExplainNode.DataSourceResult
                    };

                    LogicalSearchExplainNode.LinkNodes(filteringExplainNode, intermediateResultExplainNode);

                    intermediateResultExplainNode.RelationsList = source;

                    LogicalSearchExplainNode.LinkNodes(intermediateResultExplainNode, currentExplainNode);
                }

                return logicalSearchStorageContext.Filter(logger, source, true);
            }
        }

        private static List<RuleInstance> _emptyRuleInstancesList = new List<RuleInstance>();

        /// <inheritdoc/>
        public IList<RuleInstance> GetAllOriginFacts(IMonitorLogger logger)
        {
            lock (_lockObj)
            {
                if (_realStorageContext.Disabled)
                {
                    return _emptyRuleInstancesList;
                }

                return _factsList.ToList();
            }
        }

        private static List<BaseRulePart> _emptyBaseRulePartsList = new List<BaseRulePart>();

        /// <inheritdoc/>
        public IList<BaseRulePart> GetIndexedRulePartOfFactsByKeyOfRelation(IMonitorLogger logger, StrongIdentifierValue name, ILogicalSearchStorageContext logicalSearchStorageContext, LogicalSearchExplainNode parentExplainNode, LogicalSearchExplainNode rootParentExplainNode)
        {
            lock (_lockObj)
            {
                if (_realStorageContext.Disabled)
                {
                    return _emptyBaseRulePartsList;
                }

                LogicalSearchExplainNode currentExplainNode = null;

                if (parentExplainNode != null)
                {
                    currentExplainNode = new LogicalSearchExplainNode(rootParentExplainNode)
                    {
                        Kind = KindOfLogicalSearchExplainNode.LogicalStorage,
                        Key = name,
                        LogicalStorage = this
                    };
                }

                var source = _commonPersistIndexedLogicalData.GetIndexedRulePartOfFactsByKeyOfRelation(logger, name);

                if (logicalSearchStorageContext == null || source.IsNullOrEmpty())
                {
                    if(parentExplainNode != null)
                    {
                        LogicalSearchExplainNode.LinkNodes(parentExplainNode, currentExplainNode);
                    }

                    return source;
                }

                if(name.NormalizedNameValue == "is")
                {
                    if (parentExplainNode != null)
                    {
                        LogicalSearchExplainNode.LinkNodes(parentExplainNode, currentExplainNode);
                    }

                    return source;
                }

                LogicalSearchExplainNode filteringExplainNode = null;

                if(parentExplainNode != null)
                {
                    filteringExplainNode = new LogicalSearchExplainNode(rootParentExplainNode)
                    {
                        Kind = KindOfLogicalSearchExplainNode.LogicalStorageFilter
                    };

                    LogicalSearchExplainNode.LinkNodes(parentExplainNode, filteringExplainNode);

                    var intermediateResultExplainNode = new LogicalSearchExplainNode(rootParentExplainNode)
                    {
                        Kind = KindOfLogicalSearchExplainNode.DataSourceResult
                    };

                    LogicalSearchExplainNode.LinkNodes(filteringExplainNode, intermediateResultExplainNode);

                    intermediateResultExplainNode.BaseRulePartList = source;

                    LogicalSearchExplainNode.LinkNodes(intermediateResultExplainNode, currentExplainNode);
                }

                return logicalSearchStorageContext.Filter(logger, source, true);
            }
        }

        /// <inheritdoc/>
        public IList<BaseRulePart> GetIndexedRulePartWithOneRelationWithVarsByKeyOfRelation(IMonitorLogger logger, StrongIdentifierValue name, ILogicalSearchStorageContext logicalSearchStorageContext, LogicalSearchExplainNode parentExplainNode, LogicalSearchExplainNode rootParentExplainNode)
        {
            lock (_lockObj)
            {
                if (_realStorageContext.Disabled)
                {
                    return _emptyBaseRulePartsList;
                }

                LogicalSearchExplainNode currentExplainNode = null;

                if (parentExplainNode != null)
                {
                    currentExplainNode = new LogicalSearchExplainNode(rootParentExplainNode)
                    {
                        Kind = KindOfLogicalSearchExplainNode.LogicalStorage,
                        Key = name,
                        LogicalStorage = this
                    };
                }

                var source = _commonPersistIndexedLogicalData.GetIndexedRulePartWithOneRelationWithVarsByKeyOfRelation(logger, name);

                if (logicalSearchStorageContext == null || source.IsNullOrEmpty())
                {
                    if (parentExplainNode != null)
                    {
                        LogicalSearchExplainNode.LinkNodes(parentExplainNode, currentExplainNode);
                    }

                    return source;
                }

                LogicalSearchExplainNode filteringExplainNode = null;

                if (parentExplainNode != null)
                {
                    filteringExplainNode = new LogicalSearchExplainNode(rootParentExplainNode)
                    {
                        Kind = KindOfLogicalSearchExplainNode.LogicalStorageFilter
                    };

                    LogicalSearchExplainNode.LinkNodes(parentExplainNode, filteringExplainNode);

                    var intermediateResultExplainNode = new LogicalSearchExplainNode(rootParentExplainNode)
                    {
                        Kind = KindOfLogicalSearchExplainNode.DataSourceResult
                    };

                    LogicalSearchExplainNode.LinkNodes(filteringExplainNode, intermediateResultExplainNode);

                    intermediateResultExplainNode.BaseRulePartList = source;

                    LogicalSearchExplainNode.LinkNodes(intermediateResultExplainNode, currentExplainNode);
                }

                return logicalSearchStorageContext.Filter(logger, source, false);
            }
        }

        /// <inheritdoc/>
        public IReadOnlyList<LogicalQueryNode> GetLogicalQueryNodes(IMonitorLogger logger, IList<LogicalQueryNode> exceptList, ReplacingNotResultsStrategy replacingNotResultsStrategy, IList<KindOfLogicalQueryNode> targetKindsOfItems)
        {
            lock (_lockObj)
            {
                var result = new List<LogicalQueryNode>();

                foreach (var ruleInstance in _ruleInstancesList)
                {
                    var targetItemsList = ruleInstance.GetLogicalQueryNodes(logger, exceptList, replacingNotResultsStrategy, targetKindsOfItems);

                    if (targetItemsList.IsNullOrEmpty())
                    {
                        continue;
                    }

                    result.AddRange(targetItemsList);
                }

                return result;
            }
        }

        private RuleInstance GetRuleInstancesByStringId(string id)
        {
            return _ruleInstancesDictById.TryGetValue(id, out var ruleInstance) ? ruleInstance : null;
        }

        private void RefreshLifeTime(IMonitorLogger logger, RuleInstance ruleInstance)
        {
            NRefreshLifeTime(logger, ruleInstance.Name.NameValue);
        }

        private void RefreshLifeTime(IMonitorLogger logger, string ruleInstanceId)
        {
            NRefreshLifeTime(logger, ruleInstanceId);
        }

        private void RefreshLifeTime(IMonitorLogger logger, StrongIdentifierValue ruleInstanceName)
        {
            NRefreshLifeTime(logger, ruleInstanceName.NameValue);
        }

        private void RefreshLifeTime(IMonitorLogger logger, ulong longHashCode)
        {
            NRefreshLifeTime(logger, _ruleInstancesDictByHashCode[longHashCode].Name.NameValue);
        }

        private void NRefreshLifeTime(IMonitorLogger logger, string ruleInstanceId)
        {
            _lifeTimeCycleById[ruleInstanceId] = DEFAULT_INITIAL_TIME;

            if (_enableAddingRemovingFactLoggingInStorages)
            {
                logger.RefreshLifeTimeInLogicalStorage("5CCBE7D4-4436-4F63-9AF0-0DC060ED3468", GetRuleInstancesByStringId(ruleInstanceId).ToLabel(logger), ToLabel(logger), DEFAULT_INITIAL_TIME);
            }
        }

        private bool GCByTimeOutCommandLoop(CancellationToken cancellationToken)
        {
            Thread.Sleep(200);

            lock (_lockObj)
            {
                var logger = Logger;

                var itemsList = _lifeTimeCycleById.ToList();

                foreach(var item in itemsList)
                {
                    var lifeCycle = item.Value - 1;

                    if (lifeCycle == 0)
                    {
                        if(_enableAddingRemovingFactLoggingInStorages)
                        {
                            logger.PutFactForRemovingFromLogicalStorage("B824F12A-AD19-4F62-9849-1E1D85870B5B", GetRuleInstancesByStringId(item.Key).ToLabel(logger), ToLabel(logger));
                        }

                        NRemoveById(Logger, item.Key);
                    }
                    else
                    {
                        _lifeTimeCycleById[item.Key] = lifeCycle;
                    }
                }
            }

            return true;
        }

        /// <inheritdoc/>
        protected override void OnDisposed()
        {
            _activeObject?.Dispose();

            foreach (var parentStorage in _parentLogicalStoragesList)
            {
                parentStorage.RemoveOnChangedWithKeysHandler(this);
                parentStorage.RemoveOnChangedHandler(this);
                parentStorage.RemoveOnAddingFactHandler(this);
            }

            _parentLogicalStoragesList.Clear();

            _realStorageContext.RemoveOnAddParentStorageHandler(this);
            _realStorageContext.RemoveOnRemoveParentStorageHandler(this);

            _onChangedHandlers.Clear();
            _onChangedWithKeysHandlers.Clear();
            _onAddingFactHandlers.Clear();

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
            sb.AppendLine($"{spaces}HashCode = {GetHashCode()}");
            sb.AppendLine($"{spaces}{nameof(Kind)} = {_kind}");
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
            sb.AppendLine($"{spaces}HashCode = {GetHashCode()}");
            sb.AppendLine($"{spaces}{nameof(Kind)} = {_kind}");
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
            sb.AppendLine($"{spaces}HashCode = {GetHashCode()}");
            sb.AppendLine($"{spaces}{nameof(Kind)} = {_kind}");
            return sb.ToString();
        }

        /// <inheritdoc/>
        public string ToDbgString()
        {
            return ToDbgString(0u);
        }

        /// <inheritdoc/>
        public string ToDbgString(uint n)
        {
            return this.GetDefaultToDbgStringInformation(n);
        }

        /// <inheritdoc/>
        string IObjectToDbgString.PropertiesToDbgString(uint n)
        {
            return PropertiesToDbgString(n);
        }

        private string PropertiesToDbgString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var nextN = n + DisplayHelper.IndentationStep;
            var nextNSpaces = DisplayHelper.Spaces(nextN);

            List<RuleInstance> ruleInstancesList = null;

            lock (_lockObj)
            {
                ruleInstancesList = _ruleInstancesList.ToList();
            }

            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}({GetHashCode()}) Begin {_kind} of {_mainStorageContext.Id}");

            foreach (var ruleInstance in ruleInstancesList)
            {
                sb.Append(nextNSpaces);
                sb.AppendLine(DebugHelperForRuleInstance.ToString(ruleInstance));
            }

            sb.AppendLine($"{spaces}({GetHashCode()}) End {_kind} of {_mainStorageContext.Id}");

            return sb.ToString();
        }

        /// <inheritdoc/>
        public void DbgPrintFactsAndRules(IMonitorLogger logger)
        {
            logger.Info("5E9B4D48-9A71-4DC5-873E-19C0B90633A4", PropertiesToDbgString(0u));
        }

        /// <inheritdoc/>
        public string ToHumanizedString(HumanizedOptions options = HumanizedOptions.ShowAll)
        {
            return ToHumanizedString(DebugHelperOptions.FromHumanizedOptions(options));
        }

        /// <inheritdoc/>
        public string ToHumanizedString(DebugHelperOptions options)
        {
            return NToHumanizedLabel();
        }

        /// <inheritdoc/>
        public string ToHumanizedLabel(HumanizedOptions options = HumanizedOptions.ShowAll)
        {
            return ToHumanizedLabel(DebugHelperOptions.FromHumanizedOptions(options));
        }

        /// <inheritdoc/>
        public string ToHumanizedLabel(DebugHelperOptions options)
        {
            return NToHumanizedLabel();
        }

        private string NToHumanizedLabel()
        {
            return $"Logical storage {GetHashCode()}:{_kind}";
        }

        /// <inheritdoc/>
        public string ToHumanizedString(IMonitorLogger logger)
        {
            return ToHumanizedString();
        }

        /// <inheritdoc/>
        public MonitoredHumanizedLabel ToLabel(IMonitorLogger logger)
        {
            return new MonitoredHumanizedLabel()
            {
                Label = ToHumanizedLabel()
            };
        }
    }
}
