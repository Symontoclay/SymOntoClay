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

using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.Core.Internal.Threads;
using SymOntoClay.CoreHelper.CollectionsHelpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Monitor.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SymOntoClay.Core.Internal.Storage.LogicalStoraging
{
    public class LogicalStorage: BaseSpecificStorage, ILogicalStorage
    {
        private const int DEFAULT_INITIAL_TIME = 20;

        public LogicalStorage(KindOfStorage kind, RealStorageContext realStorageContext)
            : base(kind, realStorageContext)
        {
            var logger = realStorageContext.MainStorageContext.Logger;

            _parentLogicalStoragesList = realStorageContext.Parents.Select(p => p.LogicalStorage).ToList();

            _ruleInstancesList = new List<RuleInstance>();
            _factsList = new List<RuleInstance>();
            _ruleInstancesDict = new Dictionary<StrongIdentifierValue, RuleInstance>();
            _ruleInstancesDictByHashCode = new Dictionary<ulong, RuleInstance>();
            _ruleInstancesDictById = new Dictionary<string, RuleInstance>();
            _lifeTimeCycleById = new Dictionary<string, int>();
            _mutablePartsDict = new Dictionary<RuleInstance, IItemWithModalities>();
            _commonPersistIndexedLogicalData = new CommonPersistIndexedLogicalData(logger);
            _enableAddingRemovingFactLoggingInStorages = logger.EnableAddingRemovingFactLoggingInStorages;

            foreach (var parentStorage in _parentLogicalStoragesList)
            {
                parentStorage.OnChangedWithKeys += LogicalStorage_OnChangedWithKeys;
                parentStorage.OnChanged += LogicalStorage_OnChanged;
                parentStorage.OnAddingFact += LogicalStorage_OnAddingFact;
            }

            realStorageContext.OnAddParentStorage += RealStorageContext_OnAddParentStorage;
            realStorageContext.OnRemoveParentStorage += RealStorageContext_OnRemoveParentStorage;

            _enableOnAddingFactEvent = realStorageContext.EnableOnAddingFactEvent;

            if(_enableOnAddingFactEvent)
            {
                var mainStorageContext = realStorageContext.MainStorageContext;

                _fuzzyLogicResolver = mainStorageContext.DataResolversFactory.GetFuzzyLogicResolver();

                var localCodeExecutionContext = new LocalCodeExecutionContext(realStorageContext.ParentCodeExecutionContext);
                localCodeExecutionContext.Storage = mainStorageContext.Storage.GlobalStorage;
                localCodeExecutionContext.Holder = NameHelper.CreateName(mainStorageContext.Id);

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
        private Dictionary<RuleInstance, IItemWithModalities> _mutablePartsDict;
        private readonly bool _enableAddingRemovingFactLoggingInStorages;

        private readonly CommonPersistIndexedLogicalData _commonPersistIndexedLogicalData;
        private List<ILogicalStorage> _parentLogicalStoragesList = new List<ILogicalStorage>();

        private AsyncActivePeriodicObject _activeObject;

        private readonly bool _enableOnAddingFactEvent;

        private readonly FuzzyLogicResolver _fuzzyLogicResolver;
        private readonly ILocalCodeExecutionContext _localCodeExecutionContext;

        private void InitGCByTimeOut()
        {
            _activeObject = new AsyncActivePeriodicObject(_mainStorageContext.ActivePeriodicObjectContext);
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
            ruleInstance.CheckDirty();

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

            if(_enableOnAddingFactEvent && OnAddingFact != null)
            {
                if(isPrimary && ruleInstance.KindOfRuleInstance == KindOfRuleInstance.Fact)
                {
                    var approvingRez = AddingFactHelper.CallEvent(logger, OnAddingFact, ruleInstance, _fuzzyLogicResolver, _localCodeExecutionContext);

                    if(_enableAddingRemovingFactLoggingInStorages)
                    {
                        logger.AddFactOrRuleTriggerResult("E7837A32-70CC-4F89-9206-1DC386542546", ruleInstance.ToLabel(logger), this.ToLabel(logger), approvingRez?.ToLabel(logger));
                    }

                    if (approvingRez != null)
                    {
                        var kindOfResult = approvingRez.KindOfResult;

                        switch (kindOfResult)
                        {
                            case KindOfAddFactOrRuleResult.Reject:
                                return false;

                            case KindOfAddFactOrRuleResult.Accept:
                                if (approvingRez.MutablePart == null)
                                {
                                    break;
                                }
                                _mutablePartsDict[ruleInstance] = approvingRez.MutablePart;
                                break;

                            default:
                                throw new ArgumentOutOfRangeException(nameof(kindOfResult), kindOfResult, null);
                        }
                    }
                }
            }

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
                                    var kindOfthirdParameter = thirdParameter.Kind;
                                    switch (kindOfthirdParameter)
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
                                            throw new ArgumentOutOfRangeException(nameof(kindOfthirdParameter), kindOfthirdParameter, null);
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

            _ruleInstancesList.Remove(ruleInstance);

            _factsList.Remove(ruleInstance);

            var ruleInstanceName = ruleInstance.Name;

            _ruleInstancesDict.Remove(ruleInstanceName);

            var longHashCode = ruleInstance.GetLongHashCode();

            _ruleInstancesDictByHashCode.Remove(longHashCode);

            var ruleInstanceId = ruleInstance.Name.NameValue;

            _ruleInstancesDictById.Remove(ruleInstanceId);

            _lifeTimeCycleById.Remove(ruleInstanceId);

            _mutablePartsDict.Remove(ruleInstance);

            _commonPersistIndexedLogicalData.NRemoveIndexedRuleInstanceFromIndexData(logger, ruleInstance.Normalized);

            if(_enableAddingRemovingFactLoggingInStorages)
            {
                logger.Info("62B03746-90F3-4BE7-9E69-71BBC71583E8", $"({GetHashCode()}) `{ruleInstanceId}` {ruleInstance?.ToHumanizedLabel()} has been removed.");
            }

            return ruleInstance.UsedKeysList;
        }

        /// <inheritdoc/>
        public event Action OnChanged;

        /// <inheritdoc/>
        public event Action<IList<StrongIdentifierValue>> OnChangedWithKeys;

        /// <inheritdoc/>
        public event Func<RuleInstance, IAddFactOrRuleResult> OnAddingFact;

        protected void EmitOnChanged(IMonitorLogger logger, IList<StrongIdentifierValue> usedKeysList)
        {
            EmitOnChanged(logger);

            Task.Run(() => {
                try
                {
                    OnChangedWithKeys?.Invoke(usedKeysList);
                }
                catch (Exception e)
                {
                    logger.Error("B845E1E0-802D-4F3C-A5E6-1140F1CF7D3B", e);
                }                
            });
        }

        protected void EmitOnChanged(IMonitorLogger logger)
        {
            Task.Run(() => {
                try
                {
                    OnChanged?.Invoke();
                }
                catch (Exception e)
                {
                    logger.Error("DDC9388D-6FD6-44E6-8E24-CC13249A0914", e);
                }
            });
        }

        private void LogicalStorage_OnChangedWithKeys(IList<StrongIdentifierValue> changedKeysList)
        {
            EmitOnChanged(Logger, changedKeysList);
        }

        private void LogicalStorage_OnChanged()
        {
            EmitOnChanged(Logger);
        }

        private IAddFactOrRuleResult LogicalStorage_OnAddingFact(RuleInstance ruleInstance)
        {
            if(OnAddingFact == null)
            {
                return null;
            }

            return AddingFactHelper.CallEvent(Logger, OnAddingFact, ruleInstance, _fuzzyLogicResolver, _localCodeExecutionContext);
        }

        private void RealStorageContext_OnAddParentStorage(IStorage storage)
        {
            var logicalStorage = storage.LogicalStorage;
            logicalStorage.OnChangedWithKeys += LogicalStorage_OnChangedWithKeys;
            logicalStorage.OnChanged += LogicalStorage_OnChanged;
            logicalStorage.OnAddingFact += LogicalStorage_OnAddingFact;

            _parentLogicalStoragesList.Add(logicalStorage);
        }

        private void RealStorageContext_OnRemoveParentStorage(IStorage storage)
        {
            var logicalStorage = storage.LogicalStorage;
            logicalStorage.OnChangedWithKeys -= LogicalStorage_OnChangedWithKeys;
            logicalStorage.OnChanged -= LogicalStorage_OnChanged;
            logicalStorage.OnAddingFact -= LogicalStorage_OnAddingFact;

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

                return logicalSearchStorageContext.Filter(logger, source, true, _mutablePartsDict);
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

                return logicalSearchStorageContext.Filter(logger, source, true, _mutablePartsDict);
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
                logger.Info("5CCBE7D4-4436-4F63-9AF0-0DC060ED3468", $"({GetHashCode()}) Lifetime of `{ruleInstanceId}` {GetRuleInstancesByStringId(ruleInstanceId)?.ToHumanizedLabel()} has been refreshed to {DEFAULT_INITIAL_TIME}.");
            }
        }

        private bool GCByTimeOutCommandLoop(CancellationToken cancellationToken)
        {
            Thread.Sleep(200);

            lock (_lockObj)
            {
                var itemsList = _lifeTimeCycleById.ToList();

                foreach(var item in itemsList)
                {
                    var lifeCycle = item.Value - 1;

                    if (lifeCycle == 0)
                    {
                        if(_enableAddingRemovingFactLoggingInStorages)
                        {
                            
                            Info("B824F12A-AD19-4F62-9849-1E1D85870B5B", $"({GetHashCode()}) Put for deleting by end of life cycle: `{item.Key}` {GetRuleInstancesByStringId(item.Key)?.ToHumanizedLabel()}");
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
                parentStorage.OnChangedWithKeys -= LogicalStorage_OnChangedWithKeys;
                parentStorage.OnChanged -= LogicalStorage_OnChanged;
                parentStorage.OnAddingFact -= LogicalStorage_OnAddingFact;
            }

            _realStorageContext.OnAddParentStorage -= RealStorageContext_OnAddParentStorage;
            _realStorageContext.OnRemoveParentStorage -= RealStorageContext_OnRemoveParentStorage;

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
            var opt = new DebugHelperOptions()
            {
                HumanizedOptions = options
            };

            return ToHumanizedString(opt);
        }

        /// <inheritdoc/>
        public string ToHumanizedString(DebugHelperOptions options)
        {
            return NToHumanizedLabel();
        }

        /// <inheritdoc/>
        public string ToHumanizedLabel(HumanizedOptions options = HumanizedOptions.ShowAll)
        {
            var opt = new DebugHelperOptions()
            {
                HumanizedOptions = options
            };

            return ToHumanizedLabel(opt);
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
