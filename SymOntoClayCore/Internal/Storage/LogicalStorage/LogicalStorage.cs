using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.IndexedData;
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
        }

        private readonly KindOfStorage _kind;

        /// <inheritdoc/>
        public KindOfStorage Kind => _kind;

        private readonly RealStorageContext _realStorageContext;

        /// <inheritdoc/>
        public IStorage Storage => _realStorageContext.Storage;

        private readonly object _lockObj = new object();

        private readonly CommonPersistIndexedLogicalData _commonPersistIndexedLogicalData = new CommonPersistIndexedLogicalData();

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

            var indexedRuleInstance = ruleInstance.GetIndexed(_realStorageContext.MainStorageContext);

#if DEBUG
            Log($"indexedRuleInstance = {indexedRuleInstance}");
#endif

            _commonPersistIndexedLogicalData.NSetIndexedRuleInstanceToIndexData(indexedRuleInstance);

            //throw new NotImplementedException();

#if IMAGINE_WORKING
            //Log("End");
#else
            throw new NotImplementedException();
#endif
        }
    }
}
