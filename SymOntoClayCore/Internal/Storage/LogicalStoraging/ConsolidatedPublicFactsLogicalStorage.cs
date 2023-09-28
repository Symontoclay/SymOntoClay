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
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.CoreHelper.CollectionsHelpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.Monitor.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.Core.Internal.Storage.LogicalStoraging
{
    public class ConsolidatedPublicFactsLogicalStorage : BaseComponent, ILogicalStorage
    {
        public ConsolidatedPublicFactsLogicalStorage(ConsolidatedPublicFactsStorage parent, IMonitorLogger logger, KindOfStorage kind, ConsolidatedPublicFactsStorageSettings settings)
            : base(logger)
        {
            _kind = kind;
            _parent = parent;
            _enableOnAddingFactEvent = settings.EnableOnAddingFactEvent;

            if(_enableOnAddingFactEvent == KindOfOnAddingFactEvent.Isolated)
            {
                _mutablePartsDict = new Dictionary<RuleInstance, IItemWithModalities>();
                _rejectedFacts = new HashSet<RuleInstance>();
                _processedOnAddingFacts = new HashSet<RuleInstance>();

                var mainStorageContext = settings.MainStorageContext;

                _fuzzyLogicResolver = mainStorageContext.DataResolversFactory.GetFuzzyLogicResolver();

                var localCodeExecutionContext = new LocalCodeExecutionContext();
                localCodeExecutionContext.Storage = mainStorageContext.Storage.GlobalStorage;
                localCodeExecutionContext.Holder = NameHelper.CreateName(mainStorageContext.Id);

                _localCodeExecutionContext = localCodeExecutionContext;
            }
        }

        private readonly object _lockObj = new object();
        private readonly ConsolidatedPublicFactsStorage _parent;
        private readonly List<ILogicalStorage> _logicalStorages = new List<ILogicalStorage>();
        private Dictionary<RuleInstance, IItemWithModalities> _mutablePartsDict;
        private HashSet<RuleInstance> _rejectedFacts;
        private HashSet<RuleInstance> _processedOnAddingFacts;
        private readonly object _onAddingFactLockObj = new object();

        private KindOfOnAddingFactEvent _enableOnAddingFactEvent;

        private readonly FuzzyLogicResolver _fuzzyLogicResolver;
        private readonly ILocalCodeExecutionContext _localCodeExecutionContext;

        private readonly KindOfStorage _kind;

        /// <inheritdoc/>
        public KindOfStorage Kind => _kind;

        /// <inheritdoc/>
        public IStorage Storage => _parent;

        /// <inheritdoc/>
        public event Action OnChanged;

        /// <inheritdoc/>
        public event Action<IList<StrongIdentifierValue>> OnChangedWithKeys;

        /// <inheritdoc/>
        public event Func<RuleInstance, IAddFactOrRuleResult> OnAddingFact;

        public void AddConsolidatedStorage(IMonitorLogger logger, ILogicalStorage storage)
        {
            lock(_lockObj)
            {
                if(_logicalStorages.Contains(storage))
                {
                    return;
                }

                storage.OnChangedWithKeys += LogicalStorage_OnChangedWithKeys;

                if(_enableOnAddingFactEvent != KindOfOnAddingFactEvent.None)
                {
                    storage.OnAddingFact += LogicalStorage_OnAddingFact;

                    if(_enableOnAddingFactEvent == KindOfOnAddingFactEvent.Isolated)
                    {
                        EmitOnAddingFactForNewStorage(logger, storage);
                    }                   
                }

                _logicalStorages.Add(storage);
            }
        }

        public void RemoveConsolidatedStorage(IMonitorLogger logger, ILogicalStorage storage)
        {
            lock (_lockObj)
            {
                if (!_logicalStorages.Contains(storage))
                {
                    return;
                }

                storage.OnChangedWithKeys -= LogicalStorage_OnChangedWithKeys;

                if (_enableOnAddingFactEvent != KindOfOnAddingFactEvent.None)
                {
                    storage.OnAddingFact -= LogicalStorage_OnAddingFact;
                }

                _logicalStorages.Remove(storage);
            }
        }

        private void EmitOnChanged(IMonitorLogger logger, IList<StrongIdentifierValue> usedKeysList)
        {
            Task.Run(() => {
                try
                {
                    OnChanged?.Invoke();
                }
                catch (Exception e)
                {
                    logger.Error("6489EB1E-8E8B-4354-BC14-EA439E053431", e);
                }                
            });

            Task.Run(() => {
                try
                {
                    OnChangedWithKeys?.Invoke(usedKeysList);
                }
                catch (Exception e)
                {
                    logger.Error("EAEB1532-7C12-459C-85F6-F488CCC6F90C", e);
                }                
            });
        }

        private void LogicalStorage_OnChangedWithKeys(IList<StrongIdentifierValue> changedKeysList)
        {
            EmitOnChanged(Logger, changedKeysList);
        }

        private void EmitOnAddingFactForNewStorage(IMonitorLogger logger, ILogicalStorage storage)
        {
            Task.Run(() => {
                try
                {
                    var allFactsList = storage.GetAllOriginFacts(logger);

                    foreach(var fact in allFactsList)
                    {
                        IsolatedProcessNewFact(logger, fact);
                    }
                }
                catch (Exception e)
                {
                    logger.Error("5505F9AC-F874-4843-91D6-9CF97045326D", e);
                }
            });
        }

        private IAddFactOrRuleResult EmitIsolatedOnAddingFact(IMonitorLogger logger, RuleInstance ruleInstance)
        {
            if(OnAddingFact != null)
            {
                Task.Run(() => {
                    try
                    {
                        IsolatedProcessNewFact(logger, ruleInstance);
                    }
                    catch (Exception e)
                    {
                        logger.Error("932FA4A1-3216-4B1F-8B5E-DB7EB08A42D4", e);
                    }
                });
            }

            return new AddFactOrRuleResult() { KindOfResult = KindOfAddFactOrRuleResult.Accept };
        }

        private void IsolatedProcessNewFact(IMonitorLogger logger, RuleInstance ruleInstance)
        {
            lock(_onAddingFactLockObj)
            {
                if(_processedOnAddingFacts.Contains(ruleInstance))
                {
                    return;
                }

                _processedOnAddingFacts.Add(ruleInstance);

                var approvingRez = AddingFactHelper.CallEvent(logger, OnAddingFact, ruleInstance, _fuzzyLogicResolver, _localCodeExecutionContext);

                if (approvingRez == null)
                {
                    return;
                }

                var kindOfResult = approvingRez.KindOfResult;

                switch (kindOfResult)
                {
                    case KindOfAddFactOrRuleResult.Reject:
                        _rejectedFacts.Add(ruleInstance);
                        return;

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

        private static IAddFactOrRuleResult DefaultAddFactOrRuleResult = new AddFactOrRuleResult() { KindOfResult = KindOfAddFactOrRuleResult.Accept };

        private IAddFactOrRuleResult LogicalStorage_OnAddingFact(RuleInstance ruleInstance)
        {
            switch(_enableOnAddingFactEvent)
            {
                case KindOfOnAddingFactEvent.Transparent:
                    if(OnAddingFact == null)
                    {
                        return DefaultAddFactOrRuleResult;
                    }
                    return OnAddingFact(ruleInstance);

                case KindOfOnAddingFactEvent.Isolated:
                    return EmitIsolatedOnAddingFact(Logger, ruleInstance);

                default:
                    throw new ArgumentOutOfRangeException(nameof(_enableOnAddingFactEvent), _enableOnAddingFactEvent, null);
            }            
        }

        /// <inheritdoc/>
        public void Append(IMonitorLogger logger, RuleInstance ruleInstance)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void Append(IMonitorLogger logger, RuleInstance ruleInstance, bool isPrimary)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void Append(IMonitorLogger logger, IList<RuleInstance> ruleInstancesList)
        {
            throw new NotImplementedException();
        }

#if DEBUG
        /// <inheritdoc/>
        public void DbgPrintFactsAndRules(IMonitorLogger logger)
        {
            lock (_lockObj)
            {
                foreach (var storage in _logicalStorages)
                {
                    storage.DbgPrintFactsAndRules(logger);
                }
            }
        }
#endif

        /// <inheritdoc/>
        public IList<LogicalQueryNode> GetAllRelations(IMonitorLogger logger, ILogicalSearchStorageContext logicalSearchStorageContext, LogicalSearchExplainNode parentExplainNode, LogicalSearchExplainNode rootParentExplainNode)
        {
            lock (_lockObj)
            {
                LogicalSearchExplainNode currentExplainNode = null;

                if (parentExplainNode != null)
                {
                    currentExplainNode = new LogicalSearchExplainNode(rootParentExplainNode)
                    {
                        Kind = KindOfLogicalSearchExplainNode.LogicalStorage,
                        LogicalStorage = this
                    };
                }

                var result = new List<LogicalQueryNode>();

                switch (_enableOnAddingFactEvent)
                {
                    case KindOfOnAddingFactEvent.None:
                    case KindOfOnAddingFactEvent.Transparent:
                        foreach (var storage in _logicalStorages)
                        {
                            LogicalSearchExplainNode localResultExplainNode = null;

                            if (currentExplainNode != null)
                            {
                                LogicalSearchExplainNode.LinkNodes(parentExplainNode, currentExplainNode);

                                localResultExplainNode = new LogicalSearchExplainNode(rootParentExplainNode)
                                {
                                    Kind = KindOfLogicalSearchExplainNode.DataSourceResult
                                };

                                LogicalSearchExplainNode.LinkNodes(currentExplainNode, localResultExplainNode);
                            }

                            var targetItemsList = storage.GetAllRelations(logger, logicalSearchStorageContext, localResultExplainNode, rootParentExplainNode);

                            if (localResultExplainNode != null)
                            {
                                localResultExplainNode.RelationsList = targetItemsList;
                            }

                            if (targetItemsList.IsNullOrEmpty())
                            {
                                continue;
                            }

                            result.AddRange(targetItemsList);
                        }
                        return result;

                    case KindOfOnAddingFactEvent.Isolated:
                        {
                            LogicalSearchExplainNode filteringExplainNode = null;
                            LogicalSearchExplainNode intermediateResultExplainNode = null;

                            if (parentExplainNode != null)
                            {
                                filteringExplainNode = new LogicalSearchExplainNode(rootParentExplainNode)
                                {
                                    Kind = KindOfLogicalSearchExplainNode.LogicalStorageFilter
                                };

                                LogicalSearchExplainNode.LinkNodes(parentExplainNode, filteringExplainNode);

                                intermediateResultExplainNode = new LogicalSearchExplainNode(rootParentExplainNode)
                                {
                                    Kind = KindOfLogicalSearchExplainNode.DataSourceResult
                                };

                                LogicalSearchExplainNode.LinkNodes(filteringExplainNode, intermediateResultExplainNode);

                                LogicalSearchExplainNode.LinkNodes(intermediateResultExplainNode, currentExplainNode);
                            }

                            foreach (var storage in _logicalStorages)
                            {
                                LogicalSearchExplainNode localResultExplainNode = null;

                                if (currentExplainNode != null)
                                {
                                    localResultExplainNode = new LogicalSearchExplainNode(rootParentExplainNode)
                                    {
                                        Kind = KindOfLogicalSearchExplainNode.DataSourceResult
                                    };

                                    LogicalSearchExplainNode.LinkNodes(currentExplainNode, localResultExplainNode);
                                }

                                var targetItemsList = storage.GetAllRelations(logger, null, localResultExplainNode, rootParentExplainNode);

                                if (localResultExplainNode != null)
                                {
                                    localResultExplainNode.RelationsList = targetItemsList;
                                }

                                if (targetItemsList.IsNullOrEmpty())
                                {
                                    continue;
                                }

                                result.AddRange(targetItemsList.Where(p => !_rejectedFacts.Contains(p.RuleInstance)));
                            }

                            if (intermediateResultExplainNode != null)
                            {
                                intermediateResultExplainNode.RelationsList = result;
                            }

                            return logicalSearchStorageContext.Filter(logger, result, _mutablePartsDict);
                        }

                    default:
                        throw new ArgumentOutOfRangeException(nameof(_enableOnAddingFactEvent), _enableOnAddingFactEvent, null);
                }
            }
        }

        /// <inheritdoc/>
        public IList<RuleInstance> GetAllOriginFacts(IMonitorLogger logger)
        {
            lock (_lockObj)
            {
                var result = new List<RuleInstance>();

                foreach (var storage in _logicalStorages)
                {
                    var targetItemsList = storage.GetAllOriginFacts(logger);

                    if (targetItemsList.IsNullOrEmpty())
                    {
                        continue;
                    }

                    result.AddRange(targetItemsList);
                }

                return result;
            }
        }

        /// <inheritdoc/>
        public IList<BaseRulePart> GetIndexedRulePartOfFactsByKeyOfRelation(IMonitorLogger logger, StrongIdentifierValue name, ILogicalSearchStorageContext logicalSearchStorageContext, LogicalSearchExplainNode parentExplainNode, LogicalSearchExplainNode rootParentExplainNode)
        {
            lock (_lockObj)
            {
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

                var initialResult = new List<BaseRulePart>();

                switch (_enableOnAddingFactEvent)
                {
                    case KindOfOnAddingFactEvent.None:
                    case KindOfOnAddingFactEvent.Transparent:
                        foreach (var storage in _logicalStorages)
                        {
                            LogicalSearchExplainNode localResultExplainNode = null;

                            if (currentExplainNode != null)
                            {
                                LogicalSearchExplainNode.LinkNodes(parentExplainNode, currentExplainNode);

                                localResultExplainNode = new LogicalSearchExplainNode(rootParentExplainNode)
                                {
                                    Kind = KindOfLogicalSearchExplainNode.DataSourceResult
                                };

                                LogicalSearchExplainNode.LinkNodes(currentExplainNode, localResultExplainNode);
                            }

                            var targetItemsList = storage.GetIndexedRulePartOfFactsByKeyOfRelation(logger, name, logicalSearchStorageContext, localResultExplainNode, rootParentExplainNode);

                            if (localResultExplainNode != null)
                            {
                                localResultExplainNode.BaseRulePartList = targetItemsList;
                            }

                            if (targetItemsList.IsNullOrEmpty())
                            {
                                continue;
                            }

                            initialResult.AddRange(targetItemsList);
                        }
                        break;

                    case KindOfOnAddingFactEvent.Isolated:
                        {
                            LogicalSearchExplainNode filteringExplainNode = null;
                            LogicalSearchExplainNode intermediateResultExplainNode = null;

                            if(parentExplainNode != null)
                            {
                                filteringExplainNode = new LogicalSearchExplainNode(rootParentExplainNode)
                                {
                                    Kind = KindOfLogicalSearchExplainNode.LogicalStorageFilter
                                };

                                LogicalSearchExplainNode.LinkNodes(parentExplainNode, filteringExplainNode);

                                intermediateResultExplainNode = new LogicalSearchExplainNode(rootParentExplainNode)
                                {
                                    Kind = KindOfLogicalSearchExplainNode.DataSourceResult
                                };

                                LogicalSearchExplainNode.LinkNodes(filteringExplainNode, intermediateResultExplainNode);

                                LogicalSearchExplainNode.LinkNodes(intermediateResultExplainNode, currentExplainNode);
                            }

                            foreach (var storage in _logicalStorages)
                            {
                                LogicalSearchExplainNode localResultExplainNode = null;

                                if (currentExplainNode != null)
                                {
                                    localResultExplainNode = new LogicalSearchExplainNode(rootParentExplainNode)
                                    {
                                        Kind = KindOfLogicalSearchExplainNode.DataSourceResult
                                    };

                                    LogicalSearchExplainNode.LinkNodes(currentExplainNode, localResultExplainNode);
                                }

                                var targetItemsList = storage.GetIndexedRulePartOfFactsByKeyOfRelation(logger, name, null, localResultExplainNode, rootParentExplainNode);

                                if (localResultExplainNode != null)
                                {
                                    localResultExplainNode.BaseRulePartList = targetItemsList;
                                }

                                if (targetItemsList.IsNullOrEmpty())
                                {
                                    continue;
                                }

                                initialResult.AddRange(targetItemsList.Where(p => !_rejectedFacts.Contains(p.Parent)));
                            }

                            if(intermediateResultExplainNode != null)
                            {
                                intermediateResultExplainNode.BaseRulePartList = initialResult;
                            }

                            initialResult = logicalSearchStorageContext.Filter(logger, initialResult, _mutablePartsDict).ToList();
                        }
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(_enableOnAddingFactEvent), _enableOnAddingFactEvent, null);
                }

                if (initialResult.Count <= 1)
                {
                    return initialResult;
                }

                var result = new List<BaseRulePart>();

                var groupedDict = initialResult.GroupBy(p => p.GetLongHashCode()).ToDictionary(p => p.Key, p => p.ToList());

                foreach (var kvpItem in groupedDict)
                {
                    result.Add(kvpItem.Value.First());
                }

                return result;
            }                
        }

        /// <inheritdoc/>
        public IList<BaseRulePart> GetIndexedRulePartWithOneRelationWithVarsByKeyOfRelation(IMonitorLogger logger, StrongIdentifierValue name, ILogicalSearchStorageContext logicalSearchStorageContext, LogicalSearchExplainNode parentExplainNode, LogicalSearchExplainNode rootParentExplainNode)
        {
            lock (_lockObj)
            {
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

                var initialResult = new List<BaseRulePart>();

                switch (_enableOnAddingFactEvent)
                {
                    case KindOfOnAddingFactEvent.None:
                    case KindOfOnAddingFactEvent.Transparent:
                        foreach (var storage in _logicalStorages)
                        {
                            LogicalSearchExplainNode localResultExplainNode = null;

                            if (currentExplainNode != null)
                            {
                                LogicalSearchExplainNode.LinkNodes(parentExplainNode, currentExplainNode);

                                localResultExplainNode = new LogicalSearchExplainNode(rootParentExplainNode)
                                {
                                    Kind = KindOfLogicalSearchExplainNode.DataSourceResult
                                };

                                LogicalSearchExplainNode.LinkNodes(currentExplainNode, localResultExplainNode);
                            }

                            var targetItemsList = storage.GetIndexedRulePartWithOneRelationWithVarsByKeyOfRelation(logger, name, logicalSearchStorageContext, localResultExplainNode, rootParentExplainNode);

                            if (localResultExplainNode != null)
                            {
                                localResultExplainNode.BaseRulePartList = targetItemsList;
                            }

                            if (targetItemsList.IsNullOrEmpty())
                            {
                                continue;
                            }

                            initialResult.AddRange(targetItemsList);
                        }
                        break;

                    case KindOfOnAddingFactEvent.Isolated:
                        {
                            LogicalSearchExplainNode filteringExplainNode = null;
                            LogicalSearchExplainNode intermediateResultExplainNode = null;

                            if (parentExplainNode != null)
                            {
                                filteringExplainNode = new LogicalSearchExplainNode(rootParentExplainNode)
                                {
                                    Kind = KindOfLogicalSearchExplainNode.LogicalStorageFilter
                                };

                                LogicalSearchExplainNode.LinkNodes(parentExplainNode, filteringExplainNode);

                                intermediateResultExplainNode = new LogicalSearchExplainNode(rootParentExplainNode)
                                {
                                    Kind = KindOfLogicalSearchExplainNode.DataSourceResult
                                };

                                LogicalSearchExplainNode.LinkNodes(filteringExplainNode, intermediateResultExplainNode);

                                LogicalSearchExplainNode.LinkNodes(intermediateResultExplainNode, currentExplainNode);
                            }

                            foreach (var storage in _logicalStorages)
                            {
                                LogicalSearchExplainNode localResultExplainNode = null;

                                if (currentExplainNode != null)
                                {
                                    localResultExplainNode = new LogicalSearchExplainNode(rootParentExplainNode)
                                    {
                                        Kind = KindOfLogicalSearchExplainNode.DataSourceResult
                                    };

                                    LogicalSearchExplainNode.LinkNodes(currentExplainNode, localResultExplainNode);
                                }

                                var targetItemsList = storage.GetIndexedRulePartWithOneRelationWithVarsByKeyOfRelation(logger, name, null, localResultExplainNode, rootParentExplainNode);

                                if (localResultExplainNode != null)
                                {
                                    localResultExplainNode.BaseRulePartList = targetItemsList;
                                }

                                if (targetItemsList.IsNullOrEmpty())
                                {
                                    continue;
                                }

                                initialResult.AddRange(targetItemsList.Where(p => !_rejectedFacts.Contains(p.Parent)));
                            }

                            if (intermediateResultExplainNode != null)
                            {
                                intermediateResultExplainNode.BaseRulePartList = initialResult;
                            }

                            initialResult = logicalSearchStorageContext.Filter(logger, initialResult, false).ToList();
                        }
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(_enableOnAddingFactEvent), _enableOnAddingFactEvent, null);
                }

                if (initialResult.Count <= 1)
                {
                    return initialResult;
                }

                var result = new List<BaseRulePart>();

                var groupedDict = initialResult.GroupBy(p => p.GetLongHashCode()).ToDictionary(p => p.Key, p => p.ToList());

                foreach (var kvpItem in groupedDict)
                {
                    result.Add(kvpItem.Value.First());
                }

                return result;
            }
        }

        /// <inheritdoc/>
        public IReadOnlyList<LogicalQueryNode> GetLogicalQueryNodes(IMonitorLogger logger, IList<LogicalQueryNode> exceptList, ReplacingNotResultsStrategy replacingNotResultsStrategy, IList<KindOfLogicalQueryNode> targetKindsOfItems)
        {
            lock (_lockObj)
            {
                var result = new List<LogicalQueryNode>();

                foreach (var storage in _logicalStorages)
                {
                    var targetItemsList = storage.GetLogicalQueryNodes(logger, exceptList, replacingNotResultsStrategy, targetKindsOfItems);

                    if (targetItemsList.IsNullOrEmpty())
                    {
                        continue;
                    }

                    result.AddRange(targetItemsList);
                }

                return result;
            }
        }

        /// <inheritdoc/>
        public void Remove(IMonitorLogger logger, RuleInstance ruleInstance)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void Remove(IMonitorLogger logger, IList<RuleInstance> ruleInstancesList)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void RemoveById(IMonitorLogger logger, string id)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        protected override void OnDisposed()
        {
            lock (_lockObj)
            {
                foreach (var storage in _logicalStorages)
                {
                    storage.OnChangedWithKeys -= LogicalStorage_OnChangedWithKeys;
                }

                if (_enableOnAddingFactEvent != KindOfOnAddingFactEvent.None)
                {
                    foreach (var storage in _logicalStorages)
                    {
                        storage.OnAddingFact -= LogicalStorage_OnAddingFact;
                    }

                    if(_enableOnAddingFactEvent == KindOfOnAddingFactEvent.Isolated)
                    {
                        _mutablePartsDict.Clear();
                        _rejectedFacts.Clear();
                        _processedOnAddingFacts.Clear();
                    }
                }

                _logicalStorages.Clear();
            }

            base.OnDisposed();
        }
    }
}
