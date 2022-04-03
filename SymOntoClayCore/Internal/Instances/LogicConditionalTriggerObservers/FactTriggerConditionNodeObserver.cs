using SymOntoClay.Core.Internal.CodeModel.ConditionOfTriggerExpr;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Instances.LogicConditionalTriggerObservers
{
    public class FactTriggerConditionNodeObserver: BaseTriggerConditionNodeObserver
    {
        public FactTriggerConditionNodeObserver(IEntityLogger logger, IStorage storage)
            : base(logger)
        {
            storage.LogicalStorage.OnChanged += LogicalStorage_OnChanged;
            _storage = storage;
        }

        private readonly IStorage _storage;

        private void LogicalStorage_OnChanged()
        {
            EmitOnChanged();
        }

        /// <inheritdoc/>
        protected override void OnDisposed()
        {
            _storage.LogicalStorage.OnChanged -= LogicalStorage_OnChanged;

            base.OnDisposed();
        }
    }
}
