using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.Core.Internal.Storage.LogicalStorage
{
    public class WorldPublicFactsLogicalStorage : BaseComponent, ILogicalStorage
    {
        public WorldPublicFactsLogicalStorage(ConsolidatedPublicFactsStorage parent, IEntityLogger logger)
            : base(logger)
        {
            _parent = parent;
        }

        private readonly object _lockObj = new object();
        private readonly ConsolidatedPublicFactsStorage _parent;
        private readonly List<ILogicalStorage> _logicalStorages = new List<ILogicalStorage>();

        /// <inheritdoc/>
        public KindOfStorage Kind => KindOfStorage.WorldPublicFacts;

        /// <inheritdoc/>
        public IStorage Storage => _parent;

        /// <inheritdoc/>
        public event Action OnChanged;

        /// <inheritdoc/>
        public event Action<IList<StrongIdentifierValue>> OnChangedWithKeys;

        public void AddPublicFactsStorageOfOtherGameComponent(ILogicalStorage storage)
        {
            lock(_lockObj)
            {
                if(_logicalStorages.Contains(storage))
                {
                    return;
                }

                storage.OnChangedWithKeys += LogicalStorage_OnChangedWithKeys;

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

                _logicalStorages.Remove(storage);
            }
        }

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
        public IList<LogicalQueryNode> GetAllRelations()
        {
            lock (_lockObj)
            {
                var result = new List<LogicalQueryNode>();

                foreach(var storage in _logicalStorages)
                {
                    var targetRelationsList = storage.GetAllRelations();

                    if (targetRelationsList == null)
                    {
                        continue;
                    }

                    result.AddRange(targetRelationsList);
                }

                return result;
            }
        }

        /// <inheritdoc/>
        public IList<BaseRulePart> GetIndexedRulePartOfFactsByKeyOfRelation(StrongIdentifierValue name)
        {
            lock (_lockObj)
            {
                var initialResult = new List<BaseRulePart>();

                foreach (var storage in _logicalStorages)
                {
                    var targetRelationsList = storage.GetIndexedRulePartOfFactsByKeyOfRelation(name);

                    if (targetRelationsList == null)
                    {
                        continue;
                    }

                    initialResult.AddRange(targetRelationsList);
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
        public IList<BaseRulePart> GetIndexedRulePartWithOneRelationWithVarsByKeyOfRelation(StrongIdentifierValue name)
        {
            lock (_lockObj)
            {
                var initialResult = new List<BaseRulePart>();

                foreach (var storage in _logicalStorages)
                {
                    var targetRelationsList = storage.GetIndexedRulePartWithOneRelationWithVarsByKeyOfRelation(name);

                    if (targetRelationsList == null)
                    {
                        continue;
                    }

                    initialResult.AddRange(targetRelationsList);
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

                _logicalStorages.Clear();
            }

            base.OnDisposed();
        }
    }
}
