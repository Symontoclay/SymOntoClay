using SymOntoClay.Core.Internal.CodeModel.ConditionOfTriggerExpr;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Instances.LogicConditionalTriggerObservers
{
    public class DurationTriggerConditionNodeObserver : BaseTriggerConditionNodeObserver
    {
        public DurationTriggerConditionNodeObserver(TriggerConditionNodeObserverContext context, TriggerConditionNode condition)
            : base(context.EngineContext.Logger)
        {
#if DEBUG
            Log($"condition = {condition}");
#endif
        }
    }
}
