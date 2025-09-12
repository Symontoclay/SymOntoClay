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

namespace SymOntoClay.Core.Internal.Storage.LogicalStoraging
{
    public class ConsolidatedPublicFactsLogicalStorage : BaseComponent, ILogicalStorage,
        IOnAddingFactHandler, IOnChangedWithKeysLogicalStorageHandler
    {
        public ConsolidatedPublicFactsLogicalStorage(ConsolidatedPublicFactsStorage parent, IMonitorLogger logger, KindOfStorage kind, ConsolidatedPublicFactsStorageSettings settings)
            : base(logger)
        {
            _kind = kind;
            _parent = parent;
            _enableOnAddingFactEvent = settings.EnableOnAddingFactEvent;

            if(_enableOnAddingFactEvent == KindOfOnAddingFactEvent.Isolated)
            {
                _rejectedFacts = new HashSet<RuleInstance>();
                _processedOnAddingFacts = new HashSet<RuleInstance>();

                var mainStorageContext = settings.MainStorageContext;
                _mainStorageContext = mainStorageContext;

                _fuzzyLogicResolver = mainStorageContext.DataResolversFactory.GetFuzzyLogicResolver();

                var localCodeExecutionContext = new LocalCodeExecutionContext();
                localCodeExecutionContext.Storage = mainStorageContext.Storage.GlobalStorage;
                localCodeExecutionContext.Holder = NameHelper.CreateName(mainStorageContext.Id);

                _localCodeExecutionContext = localCodeExecutionContext;
            }
        }

        private readonly object _lockObj = new object();
        private readonly IMainStorageContext _mainStorageContext;
        private readonly ConsolidatedPublicFactsStorage _parent;
        private readonly List<ILogicalStorage> _logicalStorages = new List<ILogicalStorage>();
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

        public void AddConsolidatedStorage(IMonitorLogger logger, ILogicalStorage storage)
        {
            lock(_lockObj)
            {
                if(_logicalStorages.Contains(storage))
                {
                    return;
                }

                storage.AddOnChangedWithKeysHandler(this);

                if (_enableOnAddingFactEvent != KindOfOnAddingFactEvent.None)
                {
                    storage.AddOnAddingFactHandler(this);

                    if (_enableOnAddingFactEvent == KindOfOnAddingFactEvent.Isolated)
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

                storage.RemoveOnChangedWithKeysHandler(this);

                if (_enableOnAddingFactEvent != KindOfOnAddingFactEvent.None)
                {
                    storage.RemoveOnAddingFactHandler(this);
                }

                _logicalStorages.Remove(storage);
            }
        }

        private void EmitOnChanged(IMonitorLogger logger, IList<StrongIdentifierValue> usedKeysList)
        {
            EmitOnChangedHandlers();
            EmitOnChangedWithKeysHandlers(usedKeysList);
        }

        void IOnChangedWithKeysLogicalStorageHandler.Invoke(IList<StrongIdentifierValue> value)
        {
            LogicalStorage_OnChangedWithKeys(value);
        }

        private void LogicalStorage_OnChangedWithKeys(IList<StrongIdentifierValue> changedKeysList)
        {
            EmitOnChanged(Logger, changedKeysList);
        }

        private void EmitOnAddingFactForNewStorage(IMonitorLogger logger, ILogicalStorage storage)
        {
            ThreadTask.Run(() => {
                var taskId = logger.StartThreadTask("6EA7602B-F2EA-4204-B747-886EB25161E7");

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

                logger.StopThreadTask("A1CE76A8-4CD5-49E2-90A8-D43FA04F8AD4", taskId);
            }, _mainStorageContext.AsyncEventsThreadPool, _mainStorageContext.GetCancellationToken());
        }

        private IAddFactOrRuleResult EmitIsolatedOnAddingFact(IMonitorLogger logger, RuleInstance ruleInstance)
        {
            if(_onAddingFactHandlers.Count > 0)
            {
                ThreadTask.Run(() => {
                    var taskId = logger.StartThreadTask("612DB280-7EF8-4035-B6A5-229440E96F55");

                    try
                    {
                        IsolatedProcessNewFact(logger, ruleInstance);
                    }
                    catch (Exception e)
                    {
                        logger.Error("932FA4A1-3216-4B1F-8B5E-DB7EB08A42D4", e);
                    }

                    logger.StopThreadTask("76D7022F-2677-43F7-A1EC-519E00C60B25", taskId);
                }, _mainStorageContext.AsyncEventsThreadPool, _mainStorageContext.GetCancellationToken());
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

                var approvingRez = AddingFactHelper.CallEvent(logger, _onAddingFactHandlers, ruleInstance, _fuzzyLogicResolver, _localCodeExecutionContext);

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
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(kindOfResult), kindOfResult, null);
                }
            }            
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

        private static IAddFactOrRuleResult DefaultAddFactOrRuleResult = new AddFactOrRuleResult() { KindOfResult = KindOfAddFactOrRuleResult.Accept };

        IAddFactOrRuleResult IOnAddingFactHandler.OnAddingFact(IMonitorLogger logger, RuleInstance fact)
        {
            return LogicalStorage_OnAddingFact(logger, fact);
        }

        private IAddFactOrRuleResult LogicalStorage_OnAddingFact(IMonitorLogger logger, RuleInstance ruleInstance)
        {
            switch(_enableOnAddingFactEvent)
            {
                case KindOfOnAddingFactEvent.Transparent:
                    if(_onAddingFactHandlers.Count == 0)
                    {
                        return DefaultAddFactOrRuleResult;
                    }
                    return AddingFactHelper.CallEvent(logger, _onAddingFactHandlers, ruleInstance, _fuzzyLogicResolver, _localCodeExecutionContext);

                case KindOfOnAddingFactEvent.Isolated:
                    return EmitIsolatedOnAddingFact(Logger, ruleInstance);

                default:
                    throw new ArgumentOutOfRangeException(nameof(_enableOnAddingFactEvent), _enableOnAddingFactEvent, null);
            }            
        }

        /// <inheritdoc/>
        public void Append(IMonitorLogger logger, RuleInstance ruleInstance)
        {
            throw new NotImplementedException("6AFD7288-4850-47E9-8CC8-042A3F4CA655");
        }

        /// <inheritdoc/>
        public void Append(IMonitorLogger logger, RuleInstance ruleInstance, bool isPrimary)
        {
            throw new NotImplementedException("9E4B1544-1E75-40C7-A6C5-C71C8719C007");
        }

        /// <inheritdoc/>
        public void Append(IMonitorLogger logger, IList<RuleInstance> ruleInstancesList)
        {
            throw new NotImplementedException("6462CE04-0E22-448F-A2CC-8659B3BB3372");
        }

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

                            return logicalSearchStorageContext.Filter(logger, result);
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

                            initialResult = logicalSearchStorageContext.Filter(logger, initialResult).ToList();
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
            throw new NotImplementedException("35F8353D-6951-47E7-A864-494508F5379E");
        }

        /// <inheritdoc/>
        public void Remove(IMonitorLogger logger, IList<RuleInstance> ruleInstancesList)
        {
            throw new NotImplementedException("51381A82-96A0-435B-A7BE-36762EC694BC");
        }

        /// <inheritdoc/>
        public void RemoveById(IMonitorLogger logger, string id)
        {
            throw new NotImplementedException("30AF413F-403F-4529-BAC2-52A588507484");
        }

        /// <inheritdoc/>
        protected override void OnDisposed()
        {
            lock (_lockObj)
            {
                foreach (var storage in _logicalStorages)
                {
                    storage.RemoveOnChangedWithKeysHandler(this);
                }

                if (_enableOnAddingFactEvent != KindOfOnAddingFactEvent.None)
                {
                    foreach (var storage in _logicalStorages)
                    {
                        storage.RemoveOnAddingFactHandler(this);
                    }

                    if(_enableOnAddingFactEvent == KindOfOnAddingFactEvent.Isolated)
                    {
                        _rejectedFacts.Clear();
                        _processedOnAddingFacts.Clear();
                    }
                }

                _logicalStorages.Clear();
            }

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
            List<ILogicalStorage> logicalStorages = null;

            lock (_lockObj)
            {
                logicalStorages = logicalStorages.ToList();
            }

            var spaces = DisplayHelper.Spaces(n);
            var nextN = n + DisplayHelper.IndentationStep;

            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}({GetHashCode()}) Begin {_kind}");

            foreach (var storage in logicalStorages)
            {
                sb.AppendLine(storage.ToDbgString(nextN));
            }

            sb.AppendLine($"{spaces}({GetHashCode()}) End {_kind}");

            return sb.ToString();
        }

        /// <inheritdoc/>
        public string FactsAndRulesToDbgString()
        {
            var sb = new StringBuilder();

            List<ILogicalStorage> logicalStorages = null;

            lock (_lockObj)
            {
                logicalStorages = logicalStorages.ToList();
            }

            foreach (var storage in logicalStorages)
            {
                sb.AppendLine(storage.FactsAndRulesToDbgString());
            }

            return sb.ToString();
        }

        /// <inheritdoc/>
        public void DbgPrintFactsAndRules(IMonitorLogger logger)
        {
            List<ILogicalStorage> logicalStorages = null;

            lock (_lockObj)
            {
                logicalStorages = logicalStorages.ToList();
            }

            foreach (var storage in logicalStorages)
            {
                storage.DbgPrintFactsAndRules(logger);
            }
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
