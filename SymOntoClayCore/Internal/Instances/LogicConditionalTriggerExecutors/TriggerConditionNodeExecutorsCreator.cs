using SymOntoClay.Core.Internal.CodeExecution;
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
        public static BaseTriggerConditionNodeExecutor CreateExecutors(TriggerConditionNodeObserverContext context, LocalCodeExecutionContext localCodeExecutionContext, BindingVariables bindingVariables, TriggerConditionNode condition)
        {
            switch(condition.Kind)
            {
                case KindOfTriggerConditionNode.Fact:
                    return new FactTriggerConditionNodeExecutor(context.EngineContext, localCodeExecutionContext, condition, bindingVariables);

                case KindOfTriggerConditionNode.Duration:
                    return new DurationTriggerConditionNodeExecutor(context, condition);

                case KindOfTriggerConditionNode.BinaryOperator:
                    {
                        var result = new BinaryOperatorTriggerConditionNodeExecutor(context.EngineContext, localCodeExecutionContext, condition);
                        result.Left = CreateExecutors(context, localCodeExecutionContext, bindingVariables, condition.Left);
                        result.Right = CreateExecutors(context, localCodeExecutionContext, bindingVariables, condition.Right);
                        return result;
                    }

                case KindOfTriggerConditionNode.Var:
                    return new VarTriggerConditionNodeExecutor(context.EngineContext, localCodeExecutionContext, condition);

                case KindOfTriggerConditionNode.Value:
                    return new ValueTriggerConditionNodeExecutor(context.EngineContext.Logger, condition);

                default:
                    throw new ArgumentOutOfRangeException(nameof(condition.Kind), condition.Kind, null);
            }
        }
    }
}
