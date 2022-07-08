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
using NLog;
using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.Core.Internal.Threads;
using SymOntoClay.CoreHelper.CollectionsHelpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SymOntoClay.Core.Internal.Storage.LogicalStorage
{
    public class LogicalStorage: BaseSpecificStorage, ILogicalStorage
    {
#if DEBUG
        //private static ILogger _gbcLogger = LogManager.GetCurrentClassLogger();
#endif

        private const int DEFAULT_INITIAL_TIME = 20;

        public LogicalStorage(KindOfStorage kind, RealStorageContext realStorageContext)
            : base(kind, realStorageContext)
        {
            _parentLogicalStoragesList = realStorageContext.Parents.Select(p => p.LogicalStorage).ToList();

            _ruleInstancesList = new List<RuleInstance>();
            _ruleInstancesDict = new Dictionary<StrongIdentifierValue, RuleInstance>();
            _ruleInstancesDictByHashCode = new Dictionary<ulong, RuleInstance>();
            _ruleInstancesDictById = new Dictionary<string, RuleInstance>();
            _lifeTimeCycleById = new Dictionary<string, int>();
            _mutablePartsDict = new Dictionary<RuleInstance, MutablePartOfRuleInstance>();
            _commonPersistIndexedLogicalData = new CommonPersistIndexedLogicalData(realStorageContext.MainStorageContext.Logger);

            foreach(var parentStorage in _parentLogicalStoragesList)
            {
                parentStorage.OnChangedWithKeys += LogicalStorage_OnChangedWithKeys;
                parentStorage.OnAddingFact += LogicalStorage_OnAddingFact;
            }

            realStorageContext.OnAddParentStorage += RealStorageContext_OnAddParentStorage;
            realStorageContext.OnRemoveParentStorage += RealStorageContext_OnRemoveParentStorage;

            _enableOnAddingFactEvent = realStorageContext.EnableOnAddingFactEvent;

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
        private Dictionary<StrongIdentifierValue, RuleInstance> _ruleInstancesDict;
        private Dictionary<ulong, RuleInstance> _ruleInstancesDictByHashCode;
        private Dictionary<string, RuleInstance> _ruleInstancesDictById;
        private Dictionary<string, int> _lifeTimeCycleById;
        private Dictionary<RuleInstance, MutablePartOfRuleInstance> _mutablePartsDict;

        private readonly CommonPersistIndexedLogicalData _commonPersistIndexedLogicalData;
        private List<ILogicalStorage> _parentLogicalStoragesList = new List<ILogicalStorage>();

        private AsyncActivePeriodicObject _activeObject;

        private readonly bool _enableOnAddingFactEvent;

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

#if DEBUG
                //Log($"ruleInstance = {DebugHelperForRuleInstance.ToString(ruleInstance)}");
                //Log($"ruleInstance = {ruleInstance}");            
                //Log($"annotationsList = {annotationsList.WriteListToString()}");
#endif

                foreach (var annotationRuleInstance in annotationRuleInstancesList)
                {
                    NAppend(annotationRuleInstance, false);
                }

                //var indexedRuleInstance = ruleInstance.GetIndexed(_mainStorageContext);

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

            Log($"ruleInstance = {DebugHelperForRuleInstance.ToString(ruleInstance)}");
            //Log($"ruleInstance = {ruleInstance}");
            Log($"isPrimary = {isPrimary}");
#endif

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

            //ruleInstance = ruleInstance.Clone();

#if DEBUG
            //Log($"ruleInstance (after) = {ruleInstance}");
#endif

            //var indexedRuleInstance = ruleInstance.GetIndexed(_realStorageContext.MainStorageContext);

#if DEBUG
            //Log($"indexedRuleInstance = {indexedRuleInstance}");
#endif

            var ruleInstanceName = ruleInstance.Name;

#if DEBUG
            //Log($"ruleInstanceName = {ruleInstanceName}");
#endif

            if (_ruleInstancesDict.ContainsKey(ruleInstanceName))
            {
                RefreshLifeTime(ruleInstanceName);

                return true;
            }

            var longHashCode = ruleInstance.GetLongHashCode();

#if DEBUG
            //Log($"longHashCode = {longHashCode}");
#endif

            if(_ruleInstancesDictByHashCode.ContainsKey(longHashCode))
            {
                RefreshLifeTime(longHashCode);

                return true;
            }

            if(_enableOnAddingFactEvent && OnAddingFact != null)
            {
                if(isPrimary && ruleInstance.KindOfRuleInstance == KindOfRuleInstance.Fact)
                {
                    var approvingRez = OnAddingFact(ruleInstance);

#if DEBUG
                    Log($"approvingRez = {approvingRez}");
#endif
                    
                    if(approvingRez == null)
                    {
                        return false;
                    }

                    var kindOfResult = approvingRez.KindOfResult;

                    switch(kindOfResult)
                    {
                        case KindOfAddFactOrRuleResult.Reject:
                            return false;

                        case KindOfAddFactOrRuleResult.Accept:
                            if(approvingRez.MutablePart == null)
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

            _ruleInstancesList.Add(ruleInstance);
            _ruleInstancesDict[ruleInstanceName] = ruleInstance;
            _ruleInstancesDictByHashCode[longHashCode] = ruleInstance;
            _ruleInstancesDictById[ruleInstanceId] = ruleInstance;
            _lifeTimeCycleById[ruleInstanceId] = DEFAULT_INITIAL_TIME;

#if DEBUG
            //if(_kind == KindOfStorage.World)
            //{
            //    Log($"ruleInstance = {DebugHelperForRuleInstance.ToString(ruleInstance)}");
            //    Log($"ruleInstance.Normalized = {DebugHelperForRuleInstance.ToString(ruleInstance.Normalized)}");
            //}
            //Log($"ruleInstance = {DebugHelperForRuleInstance.ToString(ruleInstance)}");
            //Log($"ruleInstance.Normalized = {DebugHelperForRuleInstance.ToString(ruleInstance.Normalized)}");

            //if(!DebugHelperForRuleInstance.ToString(ruleInstance).Contains("is"))
            //{
            //    Log($"ruleInstance.Normalized = {DebugHelperForRuleInstance.ToString(ruleInstance.Normalized)}");
            //    Log($"ruleInstance.Normalized = {ruleInstance.Normalized}");
            //}
#endif            

            _commonPersistIndexedLogicalData.NSetIndexedRuleInstanceToIndexData(ruleInstance.Normalized);

            if(isPrimary && _kind != KindOfStorage.PublicFacts && _kind != KindOfStorage.PerceptedFacts)
            {
#if DEBUG
                //Log($"_kind = {_kind}");
#endif

                var inheritanceRelationsList = ruleInstance.GetInheritanceRelations();
                //var ruleInstanceName = ruleInstance.Name.NameValue;

                if (inheritanceRelationsList.Any())
                {
                    var inheritanceStorage = _realStorageContext.InheritanceStorage;

                    foreach (var inheritanceRelation in inheritanceRelationsList)
                    {
#if DEBUG
                        //Log($"inheritanceRelation = {inheritanceRelation}");
#endif
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
#if DEBUG
                        //Log($"inheritanceItem = {inheritanceItem}");
#endif                       

                        inheritanceStorage.SetInheritance(inheritanceItem, false);
                    }
                }
            }

#if IMAGINE_WORKING
            //Log("End");
#else
            throw new NotImplementedException();
#endif

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
#if DEBUG
            //Log($"id = {id}");
#endif

            if (_ruleInstancesDictById.ContainsKey(id))
            {
                NRemove(_ruleInstancesDictById[id]);
            }
        }

        private void NRemove(RuleInstance ruleInstance)
        {
#if DEBUG
            //Log($"ruleInstance = {ruleInstance}");
#endif

            var usedKeysList = NRemoveAndReturnUsedKeysList(ruleInstance);

            if(usedKeysList != null)
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

            var ruleInstanceName = ruleInstance.Name;

            _ruleInstancesDict.Remove(ruleInstanceName);

            var longHashCode = ruleInstance.GetLongHashCode();

            _ruleInstancesDictByHashCode.Remove(longHashCode);

            var ruleInstanceId = ruleInstance.Name.NameValue;

            _ruleInstancesDictById.Remove(ruleInstanceId);

            _lifeTimeCycleById.Remove(ruleInstanceId);

            _mutablePartsDict.Remove(ruleInstance);

            _commonPersistIndexedLogicalData.NRemoveIndexedRuleInstanceFromIndexData(ruleInstance.Normalized);

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
#if DEBUG
            //Log($"usedKeysList = {JsonConvert.SerializeObject(usedKeysList)}");
#endif

            Task.Run(() => {
                OnChanged?.Invoke();
            });

            Task.Run(() => {
                OnChangedWithKeys?.Invoke(usedKeysList);
            });
        }

        private void LogicalStorage_OnChangedWithKeys(IList<StrongIdentifierValue> changedKeysList)
        {
#if DEBUG
            //Log($"changedKeysList = {JsonConvert.SerializeObject(changedKeysList)}");
#endif

            EmitOnChanged(changedKeysList);
        }

        private IAddFactOrRuleResult LogicalStorage_OnAddingFact(RuleInstance ruleInstance)
        {
            return OnAddingFact?.Invoke(ruleInstance);
        }

        private void RealStorageContext_OnAddParentStorage(IStorage storage)
        {
            var logicalStorage = storage.LogicalStorage;
            logicalStorage.OnChangedWithKeys += LogicalStorage_OnChangedWithKeys;
            logicalStorage.OnAddingFact += LogicalStorage_OnAddingFact;

            _parentLogicalStoragesList.Add(logicalStorage);
        }

        private void RealStorageContext_OnRemoveParentStorage(IStorage storage)
        {
            var logicalStorage = storage.LogicalStorage;
            logicalStorage.OnChangedWithKeys -= LogicalStorage_OnChangedWithKeys;
            logicalStorage.OnAddingFact -= LogicalStorage_OnAddingFact;

            _parentLogicalStoragesList.Remove(logicalStorage);
        }

        /// <inheritdoc/>
        public IList<LogicalQueryNode> GetAllRelations(ILogicalSearchStorageContext logicalSearchStorageContext)
        {
            lock (_lockObj)
            {
                var source = _commonPersistIndexedLogicalData.GetAllRelations();

                if (source.IsNullOrEmpty())
                {
                    return source;
                }

#if DEBUG
                //Log("GO!!!!");
                //Log($"source.Count = {source.Count}");
#endif

#if DEBUG
                //foreach (var sourceItem in source)
                //{
                //    Log($"sourceItem.Name = '{sourceItem.Name.NameValue}'");
                //    Log($"sourceItem = {sourceItem}");
                //}
#endif

                return logicalSearchStorageContext.Filter(source, true);
            }
        }

        /// <inheritdoc/>
        public IList<BaseRulePart> GetIndexedRulePartOfFactsByKeyOfRelation(StrongIdentifierValue name, ILogicalSearchStorageContext logicalSearchStorageContext)
        {
            lock (_lockObj)
            {
#if DEBUG
                //Log($"name = {name}");
                //Log($"_kind = {_kind}");
                //Log($"GetHashCode() = {GetHashCode()}");
                //if (_kind == KindOfStorage.World)
                //{
                //    DbgPrintFactsAndRules();
                //}
#endif

                var source = _commonPersistIndexedLogicalData.GetIndexedRulePartOfFactsByKeyOfRelation(name);

#if DEBUG
                //Log($"source?.Count = {source?.Count}");
#endif

                if (source.IsNullOrEmpty())
                {
                    return source;
                }

                if(name.NormalizedNameValue == "is")
                {
                    return source;
                }

#if DEBUG
                //Log($"name = {name}");
                //Log($"_kind = {_kind}");
                //Log($"source.Count = {source.Count}");
#endif

                return logicalSearchStorageContext.Filter(source, true);
            }
        }

        /// <inheritdoc/>
        public IList<BaseRulePart> GetIndexedRulePartWithOneRelationWithVarsByKeyOfRelation(StrongIdentifierValue name, ILogicalSearchStorageContext logicalSearchStorageContext)
        {
            lock (_lockObj)
            {
#if DEBUG
                //Log($"name = {name}");
                //Log($"_kind = {_kind}");
                //if (_kind == KindOfStorage.World)
                //{
                //    DbgPrintFactsAndRules();
                //}
#endif

                var source = _commonPersistIndexedLogicalData.GetIndexedRulePartWithOneRelationWithVarsByKeyOfRelation(name);

#if DEBUG
                //Log($"source?.Count = {source?.Count}");
#endif

                if (source.IsNullOrEmpty())
                {
                    return source;
                }

#if DEBUG
                //Log($"name = {name}");
                //Log($"_kind = {_kind}");
#endif

                return logicalSearchStorageContext.Filter(source, false);
            }
        }

#if DEBUG
        /// <inheritdoc/>
        public void DbgPrintFactsAndRules()
        {
            lock (_lockObj)
            {
                var sb = new StringBuilder();
                sb.AppendLine($"Begin {_kind} of {_mainStorageContext.Id}");

                foreach (var ruleInstance in _ruleInstancesList)
                {
                    sb.AppendLine(DebugHelperForRuleInstance.ToString(ruleInstance));
                }

                sb.AppendLine($"End {_kind} of {_mainStorageContext.Id}");

                Log(sb.ToString());
            }
        }
#endif

        private void RefreshLifeTime(RuleInstance ruleInstance)
        {
#if DEBUG
            //Log($"ruleInstance = {ruleInstance}");
#endif
            NRefreshLifeTime(ruleInstance.Name.NameValue);
        }

        private void RefreshLifeTime(string ruleInstanceId)
        {
#if DEBUG
            //Log($"ruleInstanceId = {ruleInstanceId}");
#endif

            NRefreshLifeTime(ruleInstanceId);
        }

        private void RefreshLifeTime(StrongIdentifierValue ruleInstanceName)
        {
#if DEBUG
            //Log($"ruleInstanceName = {ruleInstanceName}");
#endif

            NRefreshLifeTime(ruleInstanceName.NameValue);
        }

        private void RefreshLifeTime(ulong longHashCode)
        {
#if DEBUG
            //Log($"longHashCode = {longHashCode}");
#endif

            NRefreshLifeTime(_ruleInstancesDictByHashCode[longHashCode].Name.NameValue);
        }

        private void NRefreshLifeTime(string ruleInstanceId)
        {
#if DEBUG
            //Log($"ruleInstanceId = {ruleInstanceId}");
#endif

            _lifeTimeCycleById[ruleInstanceId] = DEFAULT_INITIAL_TIME;
        }

        private bool GCByTimeOutCommandLoop()
        {
            Thread.Sleep(200);

#if DEBUG
            //Log("Do");
#endif

            lock (_lockObj)
            {
                var itemsList = _lifeTimeCycleById.ToList();

                foreach(var item in itemsList)
                {
#if DEBUG
                    //Log($"item = {item}");
#endif

                    var lifeCycle = item.Value - 1;

                    if (lifeCycle == 0)
                    {
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
                parentStorage.OnAddingFact -= LogicalStorage_OnAddingFact;
            }

            _realStorageContext.OnAddParentStorage -= RealStorageContext_OnAddParentStorage;
            _realStorageContext.OnRemoveParentStorage -= RealStorageContext_OnRemoveParentStorage;

            base.OnDisposed();
        }
    }
}
