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
            _parentLogicalStoragesList = realStorageContext.Parents.Select(p => p.LogicalStorage).ToList();

            _ruleInstancesList = new List<RuleInstance>();
            _factsList = new List<RuleInstance>();
            _ruleInstancesDict = new Dictionary<StrongIdentifierValue, RuleInstance>();
            _ruleInstancesDictByHashCode = new Dictionary<ulong, RuleInstance>();
            _ruleInstancesDictById = new Dictionary<string, RuleInstance>();
            _lifeTimeCycleById = new Dictionary<string, int>();
            _mutablePartsDict = new Dictionary<RuleInstance, IItemWithModalities>();
            _commonPersistIndexedLogicalData = new CommonPersistIndexedLogicalData(realStorageContext.MainStorageContext.Logger);
            _loggingProvider = realStorageContext.MainStorageContext.LoggingProvider;
            _enableAddingRemovingFactLoggingInStorages = _loggingProvider.EnableAddingRemovingFactLoggingInStorages;

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
        private readonly ILoggingProvider _loggingProvider;
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
        public void Append(RuleInstance ruleInstance)
        {
            Append(ruleInstance, true);
        }

        /// <inheritdoc/>
        public void Append(RuleInstance ruleInstance, bool isPrimary)
        {
            lock (_lockObj)
            {
                var usedKeysList = NAppendAndReturnUsedKeysList(ruleInstance, isPrimary);

                EmitOnChanged(usedKeysList);
            }
        }

        /// <inheritdoc/>
        public void Append(IList<RuleInstance> ruleInstancesList)
        {
            lock (_lockObj)
            {
                var usedKeysList = new List<StrongIdentifierValue>();

                foreach(var item in ruleInstancesList)
                {
                    usedKeysList.AddRange(NAppendAndReturnUsedKeysList(item, true));
                }

                usedKeysList = usedKeysList.Distinct().ToList();

                EmitOnChanged(usedKeysList);
            }
        }

        private IList<StrongIdentifierValue> NAppendAndReturnUsedKeysList(RuleInstance ruleInstance, bool isPrimary)
        {
            ruleInstance.CheckDirty();

            if (NAppend(ruleInstance, isPrimary))
            {
                var annotationsList = ruleInstance.GetAllAnnotations();

                var annotationRuleInstancesList = annotationsList.Where(p => !p.Facts.IsNullOrEmpty()).SelectMany(p => p.Facts);

                foreach (var annotationRuleInstance in annotationRuleInstancesList)
                {
                    NAppend(annotationRuleInstance, false);
                }


                return ruleInstance.Normalized.UsedKeysList.Concat(annotationRuleInstancesList.SelectMany(p => p.UsedKeysList)).Distinct().ToList();
            }

            return new List<StrongIdentifierValue>();
        }

        private bool NAppend(RuleInstance ruleInstance, bool isPrimary)
        {
#if DEBUG
            //if(!DebugHelperForRuleInstance.ToString(ruleInstance).Contains("is"))
            //{
            //Log($"ruleInstance = {DebugHelperForRuleInstance.ToString(ruleInstance)}");
            //Log($"ruleInstance = {ruleInstance}");
            //Log($"isPrimary = {isPrimary}");
            //}

            //Log($"({GetHashCode()}) ruleInstance = {DebugHelperForRuleInstance.ToString(ruleInstance)}");
            //Log($"({GetHashCode()}) ruleInstance.Normalized = {DebugHelperForRuleInstance.ToString(ruleInstance.Normalized)}");
            //Log($"ruleInstance = {ruleInstance}");
            //Log($"isPrimary = {isPrimary}");
#endif

            //if (_enableAddingRemovingFactLoggingInStorages)
            //{
            //    Log($"({GetHashCode()}) isPrimary = {isPrimary}; ruleInstance = {DebugHelperForRuleInstance.ToString(ruleInstance)}");
            //}

            if (ruleInstance.TypeOfAccess != TypeOfAccess.Local)
            {
                AnnotatedItemHelper.CheckAndFillUpHolder(ruleInstance, _realStorageContext.MainStorageContext.CommonNamesStorage);
            }

            if (_ruleInstancesList.Contains(ruleInstance))
            {
                RefreshLifeTime(ruleInstance);

                return true;
            }

            var ruleInstanceId = ruleInstance.Name.NameValue;

            if (_ruleInstancesDictById.ContainsKey(ruleInstanceId))
            {
                RefreshLifeTime(ruleInstanceId);

                return true;
            }

            var ruleInstanceName = ruleInstance.Name;

            if (_ruleInstancesDict.ContainsKey(ruleInstanceName))
            {
                RefreshLifeTime(ruleInstanceName);

                return true;
            }

            var longHashCode = ruleInstance.GetLongHashCode();

            if(_ruleInstancesDictByHashCode.ContainsKey(longHashCode))
            {
                RefreshLifeTime(longHashCode);

                return true;
            }

            if(_enableOnAddingFactEvent && OnAddingFact != null)
            {
                if(isPrimary && ruleInstance.KindOfRuleInstance == KindOfRuleInstance.Fact)
                {
                    var approvingRez = AddingFactHelper.CallEvent(OnAddingFact, ruleInstance, _fuzzyLogicResolver, _localCodeExecutionContext, Logger);

                    if(_enableAddingRemovingFactLoggingInStorages)
                    {
                        Log($"({GetHashCode()}) approvingRez = {approvingRez}");
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
                Log($"({GetHashCode()}) NEXT ruleInstance = {ruleInstance.ToHumanizedString()}");
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

            _commonPersistIndexedLogicalData.NSetIndexedRuleInstanceToIndexData(ruleInstance.Normalized, false);

            if(isPrimary && _kind != KindOfStorage.PublicFacts && _kind != KindOfStorage.PerceptedFacts)
            {
                var inheritanceRelationsList = ruleInstance.GetInheritanceRelations();

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
                        inheritanceStorage.SetInheritance(inheritanceItem, false);
                    }
                }
            }

            return true;
        }

        /// <inheritdoc/>
        public void Remove(RuleInstance ruleInstance)
        {
            lock (_lockObj)
            {
                NRemove(ruleInstance);
            }
        }

        /// <inheritdoc/>
        public void Remove(IList<RuleInstance> ruleInstancesList)
        {
            lock (_lockObj)
            {
                var usedKeysList = new List<StrongIdentifierValue>();

                foreach(var item in ruleInstancesList)
                {
                    var tmpUsedKeysList = NRemoveAndReturnUsedKeysList(item);

                    if(tmpUsedKeysList != null)
                    {
                        usedKeysList.AddRange(tmpUsedKeysList);
                    }
                }

                usedKeysList = usedKeysList.Distinct().ToList();

                EmitOnChanged(usedKeysList);
            }
        }

        /// <inheritdoc/>
        public void RemoveById(string id)
        {
            lock (_lockObj)
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    return;
                }

                NRemoveById(id);
            }
        }

        private void NRemoveById(string id)
        {
            if (_ruleInstancesDictById.ContainsKey(id))
            {
                NRemove(_ruleInstancesDictById[id]);
            }
        }

        private void NRemove(RuleInstance ruleInstance)
        {
            var usedKeysList = NRemoveAndReturnUsedKeysList(ruleInstance);

#if DEBUG
            //Log($"usedKeysList?.Count = {usedKeysList?.Count}");
#endif

            if (usedKeysList.IsNullOrEmpty())
            {
                EmitOnChanged();
            }
            else 
            { 
                EmitOnChanged(usedKeysList);
            }
        }

        private List<StrongIdentifierValue> NRemoveAndReturnUsedKeysList(RuleInstance ruleInstance)
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

            _commonPersistIndexedLogicalData.NRemoveIndexedRuleInstanceFromIndexData(ruleInstance.Normalized);

            if(_enableAddingRemovingFactLoggingInStorages)
            {
                Log($"({GetHashCode()}) `{ruleInstanceId}` has been removed.");
            }

            return ruleInstance.UsedKeysList;
        }

        /// <inheritdoc/>
        public event Action OnChanged;

        /// <inheritdoc/>
        public event Action<IList<StrongIdentifierValue>> OnChangedWithKeys;

        /// <inheritdoc/>
        public event Func<RuleInstance, IAddFactOrRuleResult> OnAddingFact;

        protected void EmitOnChanged(IList<StrongIdentifierValue> usedKeysList)
        {
            EmitOnChanged();

            Task.Run(() => {
                try
                {
                    OnChangedWithKeys?.Invoke(usedKeysList);
                }
                catch (Exception e)
                {
                    Error(e);
                }                
            });
        }

        protected void EmitOnChanged()
        {
            Task.Run(() => {
                try
                {
                    OnChanged?.Invoke();
                }
                catch (Exception e)
                {
                    Error(e);
                }
            });
        }

        private void LogicalStorage_OnChangedWithKeys(IList<StrongIdentifierValue> changedKeysList)
        {
            EmitOnChanged(changedKeysList);
        }

        private void LogicalStorage_OnChanged()
        {
            EmitOnChanged();
        }

        private IAddFactOrRuleResult LogicalStorage_OnAddingFact(RuleInstance ruleInstance)
        {
            if(OnAddingFact == null)
            {
                return null;
            }

            return AddingFactHelper.CallEvent(OnAddingFact, ruleInstance, _fuzzyLogicResolver, _localCodeExecutionContext, Logger);
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
        public IList<LogicalQueryNode> GetAllRelations(ILogicalSearchStorageContext logicalSearchStorageContext, LogicalSearchExplainNode parentExplainNode, LogicalSearchExplainNode rootParentExplainNode)
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

                var source = _commonPersistIndexedLogicalData.GetAllRelations();

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

                return logicalSearchStorageContext.Filter(source, true, _mutablePartsDict);
            }
        }

        private static List<RuleInstance> _emptyRuleInstancesList = new List<RuleInstance>();

        /// <inheritdoc/>
        public IList<RuleInstance> GetAllOriginFacts()
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
        public IList<BaseRulePart> GetIndexedRulePartOfFactsByKeyOfRelation(StrongIdentifierValue name, ILogicalSearchStorageContext logicalSearchStorageContext, LogicalSearchExplainNode parentExplainNode, LogicalSearchExplainNode rootParentExplainNode)
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

                var source = _commonPersistIndexedLogicalData.GetIndexedRulePartOfFactsByKeyOfRelation(name);

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

                return logicalSearchStorageContext.Filter(source, true, _mutablePartsDict);
            }
        }

        /// <inheritdoc/>
        public IList<BaseRulePart> GetIndexedRulePartWithOneRelationWithVarsByKeyOfRelation(StrongIdentifierValue name, ILogicalSearchStorageContext logicalSearchStorageContext, LogicalSearchExplainNode parentExplainNode, LogicalSearchExplainNode rootParentExplainNode)
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

                var source = _commonPersistIndexedLogicalData.GetIndexedRulePartWithOneRelationWithVarsByKeyOfRelation(name);

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

                return logicalSearchStorageContext.Filter(source, false);
            }
        }

        /// <inheritdoc/>
        public IReadOnlyList<LogicalQueryNode> GetLogicalQueryNodes(IList<LogicalQueryNode> exceptList, ReplacingNotResultsStrategy replacingNotResultsStrategy, IList<KindOfLogicalQueryNode> targetKindsOfItems)
        {
            lock (_lockObj)
            {
#if DEBUG
                Log($"exceptList = {exceptList.WriteListToString()}");
                Log($"replacingNotResultsStrategy = {replacingNotResultsStrategy}");
                Log($"targetKindsOfItems = {targetKindsOfItems.WritePODListToString()}");
#endif

                var result = new List<LogicalQueryNode>();

                foreach (var ruleInstance in _ruleInstancesList)
                {
                    var targetItemsList = ruleInstance.GetLogicalQueryNodes(exceptList, replacingNotResultsStrategy, targetKindsOfItems);

                    if (targetItemsList.IsNullOrEmpty())
                    {
                        continue;
                    }

                    result.AddRange(targetItemsList);
                }

                return result;
            }
        }

#if DEBUG
        /// <inheritdoc/>
        public void DbgPrintFactsAndRules()
        {
            lock (_lockObj)
            {
                var sb = new StringBuilder();
                sb.AppendLine($"({GetHashCode()}) Begin {_kind} of {_mainStorageContext.Id}");

                foreach (var ruleInstance in _ruleInstancesList)
                {
                    sb.AppendLine(DebugHelperForRuleInstance.ToString(ruleInstance));
                }

                sb.AppendLine($"({GetHashCode()}) End {_kind} of {_mainStorageContext.Id}");

                Log(sb.ToString());
            }
        }
#endif

        private void RefreshLifeTime(RuleInstance ruleInstance)
        {
            NRefreshLifeTime(ruleInstance.Name.NameValue);
        }

        private void RefreshLifeTime(string ruleInstanceId)
        {
            NRefreshLifeTime(ruleInstanceId);
        }

        private void RefreshLifeTime(StrongIdentifierValue ruleInstanceName)
        {
            NRefreshLifeTime(ruleInstanceName.NameValue);
        }

        private void RefreshLifeTime(ulong longHashCode)
        {
            NRefreshLifeTime(_ruleInstancesDictByHashCode[longHashCode].Name.NameValue);
        }

        private void NRefreshLifeTime(string ruleInstanceId)
        {
            _lifeTimeCycleById[ruleInstanceId] = DEFAULT_INITIAL_TIME;

            if (_enableAddingRemovingFactLoggingInStorages)
            {
                Log($"({GetHashCode()}) Lifetime of `{ruleInstanceId}` has been refreshed to {DEFAULT_INITIAL_TIME}.");
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
                            Log($"({GetHashCode()}) Put for deleting by end of life cycle: `{item}`");
                        }

                        NRemoveById(item.Key);
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
    }
}
