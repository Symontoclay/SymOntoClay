using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.ConditionOfTriggerExpr;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Instances.LogicConditionalTriggerObservers
{
    public class TriggerNameTriggerConditionNodeObserver : BaseTriggerConditionNodeObserver
    {
        public TriggerNameTriggerConditionNodeObserver(IEntityLogger logger, IStorage storage, TriggerConditionNode condition)
            : base(logger)
        {
            _triggerName = condition.Name;

#if DEBUG
            //Log($"_triggerName = {_triggerName}");
#endif

            storage.TriggersStorage.OnNamedTriggerInstanceChangedWithKeys += TriggersStorage_OnNamedTriggerInstanceChangedWithKeys;
            _storage = storage;
        }

        private readonly StrongIdentifierValue _triggerName;
        private readonly IStorage _storage;

        private void TriggersStorage_OnNamedTriggerInstanceChangedWithKeys(IList<StrongIdentifierValue> namesList)
        {
#if DEBUG
            //Log($"namesList = {namesList.WriteListToString()}");
            //Log($"_triggerName = {_triggerName}");
#endif

            if(namesList.Contains(_triggerName))
            {
                EmitOnChanged();
            }
        }

        /// <inheritdoc/>
        protected override void OnDisposed()
        {
            _storage.TriggersStorage.OnNamedTriggerInstanceChangedWithKeys -= TriggersStorage_OnNamedTriggerInstanceChangedWithKeys;

            base.OnDisposed();
        }
    }
}
