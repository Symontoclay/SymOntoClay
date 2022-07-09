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

using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.CoreHelper.CollectionsHelpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.Core.Internal.Storage.LogicalStorage
{
    public class ConsolidatedPublicFactsLogicalStorage : BaseComponent, ILogicalStorage
    {
        public ConsolidatedPublicFactsLogicalStorage(ConsolidatedPublicFactsStorage parent, IEntityLogger logger, ConsolidatedPublicFactsStorageSettings settings)
            : base(logger)
        {
            _parent = parent;
            _enableOnAddingFactEvent = settings.EnableOnAddingFactEvent;

            if(_enableOnAddingFactEvent == KindOfOnAddingFactEvent.Isolated)
            {
                _mutablePartsDict = new Dictionary<RuleInstance, IItemWithModalities>();
                _rejectedFacts = new HashSet<RuleInstance>();
                _processedOnAddingFacts = new HashSet<RuleInstance>();
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

        /// <inheritdoc/>
        public KindOfStorage Kind => KindOfStorage.WorldPublicFacts;

        /// <inheritdoc/>
        public IStorage Storage => _parent;

        /// <inheritdoc/>
        public event Action OnChanged;

        /// <inheritdoc/>
        public event Action<IList<StrongIdentifierValue>> OnChangedWithKeys;

        /// <inheritdoc/>
        public event Func<RuleInstance, IAddFactOrRuleResult> OnAddingFact;

        public void AddPublicFactsStorageOfOtherGameComponent(ILogicalStorage storage)
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
                        EmitOnAddingFactForNewStorage(storage);
                    }                   
                }

                _logicalStorages.Add(storage);
            }
        }

        public void RemovePublicFactsStorageOfOtherGameComponent(ILogicalStorage storage)
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

        private void EmitOnChanged(IList<StrongIdentifierValue> usedKeysList)
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

        private void EmitOnAddingFactForNewStorage(ILogicalStorage storage)
        {
            Task.Run(() => {
                try
                {
                    var allFactsList = storage.GetAllOriginFacts();

                    foreach(var fact in allFactsList)
                    {
                        IsolatedProcessNewFact(fact);
                    }
                }
                catch (Exception e)
                {
                    Log(e.ToString());
                }
            });
        }

        private IAddFactOrRuleResult EmitIsolatedOnAddingFact(RuleInstance ruleInstance)
        {
            if(OnAddingFact != null)
            {
                Task.Run(() => {
                    try
                    {
                        IsolatedProcessNewFact(ruleInstance);
                    }
                    catch (Exception e)
                    {
                        Log(e.ToString());
                    }
                });
            }

            return new AddFactOrRuleResult() { KindOfResult = KindOfAddFactOrRuleResult.Accept };
        }

        private void IsolatedProcessNewFact(RuleInstance ruleInstance)
        {
#if DEBUG
            //Log($"ruleInstance = {ruleInstance.ToHumanizedString()}");
#endif

            lock(_onAddingFactLockObj)
            {
                if(_processedOnAddingFacts.Contains(ruleInstance))
                {
                    return;
                }

                _processedOnAddingFacts.Add(ruleInstance);

                var approvingRez = OnAddingFact(ruleInstance);

#if DEBUG
                //Log($"approvingRez = {approvingRez}");
#endif

                if (approvingRez == null)
                {
                    _rejectedFacts.Add(ruleInstance);

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
                    return EmitIsolatedOnAddingFact(ruleInstance);

                default:
                    throw new ArgumentOutOfRangeException(nameof(_enableOnAddingFactEvent), _enableOnAddingFactEvent, null);
            }            
        }

        /// <inheritdoc/>
        public void Append(RuleInstance ruleInstance)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void Append(RuleInstance ruleInstance, bool isPrimary)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void Append(IList<RuleInstance> ruleInstancesList)
        {
            throw new NotImplementedException();
        }

#if DEBUG
        /// <inheritdoc/>
        public void DbgPrintFactsAndRules()
        {
            lock (_lockObj)
            {
                foreach (var storage in _logicalStorages)
                {
                    storage.DbgPrintFactsAndRules();
                }
            }
        }
#endif

        /// <inheritdoc/>
        public IList<LogicalQueryNode> GetAllRelations(ILogicalSearchStorageContext logicalSearchStorageContext)
        {
            lock (_lockObj)
            {
                var result = new List<LogicalQueryNode>();

                switch (_enableOnAddingFactEvent)
                {
                    case KindOfOnAddingFactEvent.None:
                    case KindOfOnAddingFactEvent.Transparent:
                        foreach (var storage in _logicalStorages)
                        {
                            var targetItemsList = storage.GetAllRelations(logicalSearchStorageContext);

                            if (targetItemsList.IsNullOrEmpty())
                            {
                                continue;
                            }

                            result.AddRange(targetItemsList);
                        }
                        break;

                    case KindOfOnAddingFactEvent.Isolated:
                        foreach (var storage in _logicalStorages)
                        {
                            var targetItemsList = storage.GetAllRelations(null);

                            if (targetItemsList.IsNullOrEmpty())
                            {
                                continue;
                            }

                            result.AddRange(targetItemsList.Where(p => !_rejectedFacts.Contains(p.RuleInstance)));
                        }
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(_enableOnAddingFactEvent), _enableOnAddingFactEvent, null);
                }

                switch (_enableOnAddingFactEvent)
                {
                    case KindOfOnAddingFactEvent.None:
                    case KindOfOnAddingFactEvent.Transparent:
                        return result;

                    case KindOfOnAddingFactEvent.Isolated:
                        return logicalSearchStorageContext.Filter(result, _mutablePartsDict);

                    default:
                        throw new ArgumentOutOfRangeException(nameof(_enableOnAddingFactEvent), _enableOnAddingFactEvent, null);
                }
            }
        }

        /// <inheritdoc/>
        public IList<RuleInstance> GetAllOriginFacts()
        {
            lock (_lockObj)
            {
                var result = new List<RuleInstance>();

                foreach (var storage in _logicalStorages)
                {
                    var targetItemsList = storage.GetAllOriginFacts();

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
        public IList<BaseRulePart> GetIndexedRulePartOfFactsByKeyOfRelation(StrongIdentifierValue name, ILogicalSearchStorageContext logicalSearchStorageContext)
        {
            lock (_lockObj)
            {
                var initialResult = new List<BaseRulePart>();

                switch (_enableOnAddingFactEvent)
                {
                    case KindOfOnAddingFactEvent.None:
                    case KindOfOnAddingFactEvent.Transparent:
                        foreach (var storage in _logicalStorages)
                        {
                            var targetItemsList = storage.GetIndexedRulePartOfFactsByKeyOfRelation(name, logicalSearchStorageContext);

                            if (targetItemsList.IsNullOrEmpty())
                            {
                                continue;
                            }

                            initialResult.AddRange(targetItemsList);
                        }
                        break;

                    case KindOfOnAddingFactEvent.Isolated:
                        foreach (var storage in _logicalStorages)
                        {
                            var targetItemsList = storage.GetIndexedRulePartOfFactsByKeyOfRelation(name, null);

                            if (targetItemsList.IsNullOrEmpty())
                            {
                                continue;
                            }

                            initialResult.AddRange(targetItemsList.Where(p => !_rejectedFacts.Contains(p.Parent)));
                        }

                        initialResult = logicalSearchStorageContext.Filter(initialResult, _mutablePartsDict).ToList();
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
        public IList<BaseRulePart> GetIndexedRulePartWithOneRelationWithVarsByKeyOfRelation(StrongIdentifierValue name, ILogicalSearchStorageContext logicalSearchStorageContext)
        {
            lock (_lockObj)
            {
                var initialResult = new List<BaseRulePart>();

                switch (_enableOnAddingFactEvent)
                {
                    case KindOfOnAddingFactEvent.None:
                    case KindOfOnAddingFactEvent.Transparent:
                        foreach (var storage in _logicalStorages)
                        {
                            var targetItemsList = storage.GetIndexedRulePartWithOneRelationWithVarsByKeyOfRelation(name, logicalSearchStorageContext);

                            if (targetItemsList.IsNullOrEmpty())
                            {
                                continue;
                            }

                            initialResult.AddRange(targetItemsList);
                        }
                        break;

                    case KindOfOnAddingFactEvent.Isolated:
                        foreach (var storage in _logicalStorages)
                        {
                            var targetItemsList = storage.GetIndexedRulePartWithOneRelationWithVarsByKeyOfRelation(name, null);

                            if (targetItemsList.IsNullOrEmpty())
                            {
                                continue;
                            }

                            initialResult.AddRange(targetItemsList.Where(p => !_rejectedFacts.Contains(p.Parent)));
                        }

                        initialResult = logicalSearchStorageContext.Filter(initialResult, false).ToList();
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
        public void Remove(RuleInstance ruleInstance)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void Remove(IList<RuleInstance> ruleInstancesList)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void RemoveById(string id)
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