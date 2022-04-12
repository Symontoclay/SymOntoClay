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
            Log($"_triggerName = {_triggerName}");
#endif
        }

        private readonly StrongIdentifierValue _triggerName;
        private readonly IStorage _storage;
    }
}
