using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Ast.Expressions;
using SymOntoClay.Core.Internal.CodeModel.ConditionOfTriggerExpr;
using SymOntoClay.Core.Internal.Instances.LogicConditionalTriggerObservers;
using SymOntoClay.CoreHelper.CollectionsHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Instances.LogicConditionalTriggerExecutors
{
    public static class TriggerConditionNodeExecutorsCreator
    {
        public static BaseTriggerConditionNodeExecutor CreateExecutors(TriggerConditionNodeObserverContext context, LocalCodeExecutionContext localCodeExecutionContext, BindingVariables bindingVariables, TriggerConditionNode condition)
        {
            return CreateExecutors(context, localCodeExecutionContext, bindingVariables, condition, true);
        }

        public static BaseTriggerConditionNodeExecutor CreateExecutors(TriggerConditionNodeObserverContext context, LocalCodeExecutionContext localCodeExecutionContext, BindingVariables bindingVariables, TriggerConditionNode condition, bool conceptAsTriggerName)
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
                        result.Left = CreateExecutors(context, localCodeExecutionContext, bindingVariables, condition.Left, conceptAsTriggerName);
                        result.Right = CreateExecutors(context, localCodeExecutionContext, bindingVariables, condition.Right, conceptAsTriggerName);
                        return result;
                    }

                case KindOfTriggerConditionNode.UnaryOperator:
                    {
                        var result = new UnaryOperatorTriggerConditionNodeExecutor(context.EngineContext, localCodeExecutionContext, condition);

                        if (condition.KindOfOperator == KindOfOperator.CallFunction)
                        {
                            if (!condition.ParamsList.IsNullOrEmpty())
                            {
                                var paramsList = new List<BaseTriggerConditionNodeExecutor>();

                                result.ParamsList = paramsList;

                                foreach (var param in condition.ParamsList)
                                {
                                    paramsList.Add(CreateExecutors(context, localCodeExecutionContext, bindingVariables, param, false));
                                }
                            }

                            result.Left = CreateExecutors(context, localCodeExecutionContext, bindingVariables, condition.Left, false);
                        }
                        else
                        {
                            result.Left = CreateExecutors(context, localCodeExecutionContext, bindingVariables, condition.Left, conceptAsTriggerName);
                        }
                        
                        return result;
                    }

                case KindOfTriggerConditionNode.Var:
                    return new VarTriggerConditionNodeExecutor(context.EngineContext, localCodeExecutionContext, condition);

                case KindOfTriggerConditionNode.Value:
                    return new ValueTriggerConditionNodeExecutor(context.EngineContext.Logger, condition);

                case KindOfTriggerConditionNode.Concept:
                    if(conceptAsTriggerName)
                    {
                        return new TriggerNameTriggerConditionNodeExecutor(context.EngineContext, localCodeExecutionContext, condition);
                    }
                    return new ConceptTriggerConditionNodeExecutor(context.EngineContext.Logger, condition);

                default:
                    throw new ArgumentOutOfRangeException(nameof(condition.Kind), condition.Kind, null);
            }
        }
    }
}
