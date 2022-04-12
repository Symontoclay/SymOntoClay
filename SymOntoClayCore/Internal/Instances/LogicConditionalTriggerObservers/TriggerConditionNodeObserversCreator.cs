using SymOntoClay.Core.Internal.CodeModel.Ast.Expressions;
using SymOntoClay.Core.Internal.CodeModel.ConditionOfTriggerExpr;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Instances.LogicConditionalTriggerObservers
{
    public static class TriggerConditionNodeObserversCreator
    {
        public static List<BaseTriggerConditionNodeObserver> CreateObservers(TriggerConditionNodeObserverContext context, TriggerConditionNode condition)
        {
            var result = new List<BaseTriggerConditionNodeObserver>();

            CreateObservers(result, context, condition);

            return result;
        }

        private static void CreateObservers(List<BaseTriggerConditionNodeObserver> result, TriggerConditionNodeObserverContext context, TriggerConditionNode condition)
        {
            switch (condition.Kind)
            {
                case KindOfTriggerConditionNode.Fact:
                    result.Add(new FactTriggerConditionNodeObserver(context.EngineContext.Logger, context.Storage));
                    break;

                case KindOfTriggerConditionNode.Duration:
                    result.Add(new DurationTriggerConditionNodeObserver(context, condition));
                    break;

                case KindOfTriggerConditionNode.Var:
                    result.Add(new VarTriggerConditionNodeObserver(context.EngineContext.Logger, context.Storage, condition));
                    break;

                case KindOfTriggerConditionNode.Value:
                    break;

                case KindOfTriggerConditionNode.BinaryOperator:
                    CreateObservers(result, context, condition.Left);
                    CreateObservers(result, context, condition.Right);
                    break;

                case KindOfTriggerConditionNode.UnaryOperator:
                    result.Add(new UnaryOperatorTriggerConditionNodeObserver(context.EngineContext.Logger, context.Storage, condition));

                    if(condition.KindOfOperator != KindOfOperator.CallFunction)
                    {
                        CreateObservers(result, context, condition.Left);
                    }
                    break;

                case KindOfTriggerConditionNode.Group:
                    CreateObservers(result, context, condition.Left);
                    break;

                case KindOfTriggerConditionNode.Concept:
                    result.Add(new TriggerNameTriggerConditionNodeObserver(context.EngineContext.Logger, context.Storage, condition));
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(condition.Kind), condition.Kind, null);
            }
        }
    }
}
