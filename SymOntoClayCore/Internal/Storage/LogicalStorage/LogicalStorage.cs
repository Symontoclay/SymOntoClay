using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Storage.LogicalStorage
{
    public class LogicalStorage: BaseLoggedComponent, ILogicalStorage
    {
        public LogicalStorage(KindOfStorage kind, RealStorageContext realStorageContext)
            : base(realStorageContext.MainStorageContext.Logger)
        {
            _kind = kind;
            _realStorageContext = realStorageContext;
            _ruleInstancesList = new List<RuleInstance>();
            _ruleInstancesDict = new Dictionary<ulong, RuleInstance>();
            _commonPersistIndexedLogicalData = new CommonPersistIndexedLogicalData(realStorageContext.MainStorageContext.Logger, realStorageContext.MainStorageContext.Dictionary);
        }

        private readonly KindOfStorage _kind;

        /// <inheritdoc/>
        public KindOfStorage Kind => _kind;

        private readonly RealStorageContext _realStorageContext;

        /// <inheritdoc/>
        public IStorage Storage => _realStorageContext.Storage;

        private readonly object _lockObj = new object();

        private List<RuleInstance> _ruleInstancesList;
        private Dictionary<ulong, RuleInstance> _ruleInstancesDict;
        private readonly CommonPersistIndexedLogicalData _commonPersistIndexedLogicalData;

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
                var annotationsList = ruleInstance.GetAllAnnotations();

#if DEBUG
                Log($"annotationsList = {annotationsList.WriteListToString()}");
#endif

                foreach(var annotationRuleInstance in annotationsList)
                {
                    NAppend(annotationRuleInstance, false);
                }

                NAppend(ruleInstance, isPrimary);
            }
        }

        private void NAppend(RuleInstance ruleInstance, bool isPrimary)
        {
#if DEBUG
            //Log($"ruleInstance = {ruleInstance}");
            //Log($"isPrimary = {isPrimary}");
            Log($"ruleInstance = {DebugHelperForRuleInstance.ToString(ruleInstance)}");
#endif

            if (_ruleInstancesList.Contains(ruleInstance))
            {
                return;
            }

            //ruleInstance = ruleInstance.Clone();

#if DEBUG
            Log($"ruleInstance (after) = {ruleInstance}");
#endif

            var indexedRuleInstance = ruleInstance.GetIndexed(_realStorageContext.MainStorageContext);

#if DEBUG
            Log($"indexedRuleInstance = {indexedRuleInstance}");
#endif

            var ruleInstanceKey = indexedRuleInstance.Key;

#if DEBUG
            Log($"ruleInstanceKey = {ruleInstanceKey}");
#endif

            if (_ruleInstancesDict.ContainsKey(ruleInstanceKey))
            {
                return;
            }

            _ruleInstancesList.Add(ruleInstance);
            _ruleInstancesDict[ruleInstanceKey] = ruleInstance;

            _commonPersistIndexedLogicalData.NSetIndexedRuleInstanceToIndexData(indexedRuleInstance);

            EmitOnChanged();

#if IMAGINE_WORKING
            Log("End");
#else
            throw new NotImplementedException();
#endif
        }

        /// <inheritdoc/>
        public event Action OnChanged;

        protected void EmitOnChanged()
        {
            OnChanged?.Invoke();
        }

        /// <inheritdoc/>
        public IList<RelationIndexedLogicalQueryNode> GetAllRelations()
        {
            lock (_lockObj)
            {
                return _commonPersistIndexedLogicalData.GetAllRelations();
            }
        }

        /// <inheritdoc/>
        public IList<IndexedBaseRulePart> GetIndexedRulePartOfFactsByKeyOfRelation(ulong key)
        {
            lock (_lockObj)
            {
#if DEBUG
                //LogInstance.Log($"key = {key}");
#endif

                return _commonPersistIndexedLogicalData.GetIndexedRulePartOfFactsByKeyOfRelation(key);
            }
        }
    }
}
