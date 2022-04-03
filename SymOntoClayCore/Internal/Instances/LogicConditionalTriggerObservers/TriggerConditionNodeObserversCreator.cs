using SymOntoClay.Core.Internal.CodeModel.ConditionOfTriggerExpr;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Instances.LogicConditionalTriggerObservers
{
    public static class TriggerConditionNodeObserversCreator
    {
        public static List<BaseTriggerConditionNodeObserver> CreateObservers(IEngineContext context, IStorage storage, TriggerConditionNode condition)
        {
            var result = new List<BaseTriggerConditionNodeObserver>();

            CreateObservers(result, context, storage, condition);

            return result;
        }

        private static void CreateObservers(List<BaseTriggerConditionNodeObserver> result, IEngineContext context, IStorage storage, TriggerConditionNode condition)
        {
            switch (condition.Kind)
            {
                case KindOfTriggerConditionNode.Fact:
                    result.Add(new FactTriggerConditionNodeObserver(context.Logger, storage));
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(condition.Kind), condition.Kind, null);
            }
        }
    }
}
