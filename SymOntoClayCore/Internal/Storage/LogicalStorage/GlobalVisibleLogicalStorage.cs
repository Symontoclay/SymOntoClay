using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Storage.LogicalStorage
{
    public class GlobalVisibleLogicalStorage : BaseComponent, ILogicalStorage
    {
        public GlobalVisibleLogicalStorage()
        {
        }

        #region ILogicalStorage
        /// <inheritdoc/>
        KindOfStorage ISpecificStorage.Kind => KindOfStorage.GlobalVisible;

        /// <inheritdoc/>
        IStorage ISpecificStorage.Storage => this;

        /// <inheritdoc/>
        void ILogicalStorage.Append(RuleInstance ruleInstance) => throw new NotImplementedException();

        /// <inheritdoc/>
        void ILogicalStorage.Append(RuleInstance ruleInstance, bool isPrimary) => throw new NotImplementedException();

        /// <inheritdoc/>
        void ILogicalStorage.Append(IList<RuleInstance> ruleInstancesList) => throw new NotImplementedException();

        /// <inheritdoc/>
        void ILogicalStorage.Remove(RuleInstance ruleInstance) => throw new NotImplementedException();

        /// <inheritdoc/>
        void ILogicalStorage.Remove(IList<RuleInstance> ruleInstancesList) => throw new NotImplementedException();

        /// <inheritdoc/>
        void ILogicalStorage.RemoveById(string id) => throw new NotImplementedException();

        /// <inheritdoc/>
        public event Action OnChanged;

        /// <inheritdoc/>
        public event Action<IList<StrongIdentifierValue>> OnChangedWithKeys;

        /// <inheritdoc/>
        public event Func<RuleInstance, IAddFactOrRuleResult> OnAddingFact;

        /// <inheritdoc/>
        IList<LogicalQueryNode> ILogicalStorage.GetAllRelations(ILogicalSearchStorageContext logicalSearchStorageContext)
        {
            return _commonPersistIndexedLogicalData.GetAllRelations();
        }

        /// <inheritdoc/>
        IList<BaseRulePart> ILogicalStorage.GetIndexedRulePartOfFactsByKeyOfRelation(StrongIdentifierValue name, ILogicalSearchStorageContext logicalSearchStorageContext)
        {
#if DEBUG
            //LogInstance.Log($"key = {key}");
#endif

            return _commonPersistIndexedLogicalData.GetIndexedRulePartOfFactsByKeyOfRelation(name);
        }

        /// <inheritdoc/>
        IList<BaseRulePart> ILogicalStorage.GetIndexedRulePartWithOneRelationWithVarsByKeyOfRelation(StrongIdentifierValue name, ILogicalSearchStorageContext logicalSearchStorageContext)
        {
            return _commonPersistIndexedLogicalData.GetIndexedRulePartWithOneRelationWithVarsByKeyOfRelation(name);
        }

        /// <inheritdoc/>
        void ILogicalStorage.DbgPrintFactsAndRules() => throw new NotImplementedException();
        #endregion

    }
}
