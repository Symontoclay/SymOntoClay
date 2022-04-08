using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.ConditionOfTriggerExpr;
using SymOntoClay.Core.Internal.Instances.LogicConditionalTriggerObservers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Instances.LogicConditionalTriggerExecutors
{
    public static class TriggerConditionNodeExecutorsCreator
    {
        public static BaseTriggerConditionNodeExecutor CreateExecutors(TriggerConditionNodeObserverContext context, BindingVariables bindingVariables, TriggerConditionNode condition)
        {
            switch(condition.Kind)
            {
                case KindOfTriggerConditionNode.Fact:
                    return new FactTriggerConditionNodeExecutor(context.EngineContext, context.Storage, context.Holder, condition, bindingVariables);

                case KindOfTriggerConditionNode.Duration:
                    return new DurationTriggerConditionNodeExecutor(context, condition);

                default:
                    throw new ArgumentOutOfRangeException(nameof(condition.Kind), condition.Kind, null);
            }
        }
    }
}
