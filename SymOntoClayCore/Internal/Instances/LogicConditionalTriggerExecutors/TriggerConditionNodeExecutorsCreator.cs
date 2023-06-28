/*MIT License

Copyright (c) 2020 - 2023 Sergiy Tolkachov

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.*/

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
        public static BaseTriggerConditionNodeExecutor CreateExecutors(TriggerConditionNodeObserverContext context, ILocalCodeExecutionContext localCodeExecutionContext, BindingVariables bindingVariables, TriggerConditionNode condition, KindOfTriggerCondition kindOfTriggerCondition)
        {
            return CreateExecutors(context, localCodeExecutionContext, bindingVariables, condition, kindOfTriggerCondition, true);
        }

        public static BaseTriggerConditionNodeExecutor CreateExecutors(TriggerConditionNodeObserverContext context, ILocalCodeExecutionContext localCodeExecutionContext, BindingVariables bindingVariables, TriggerConditionNode condition, KindOfTriggerCondition kindOfTriggerCondition, bool conceptAsTriggerName)
        {
            switch(condition.Kind)
            {
                case KindOfTriggerConditionNode.Fact:
                    return new FactTriggerConditionNodeExecutor(context.EngineContext, localCodeExecutionContext, condition, kindOfTriggerCondition, bindingVariables);

                case KindOfTriggerConditionNode.Duration:
                    return new DurationTriggerConditionNodeExecutor(context, condition, kindOfTriggerCondition);

                case KindOfTriggerConditionNode.Each:
                    return new EachTriggerConditionNodeExecutor(context, condition, kindOfTriggerCondition);

                case KindOfTriggerConditionNode.BinaryOperator:
                    {
                        var result = new BinaryOperatorTriggerConditionNodeExecutor(context.EngineContext, localCodeExecutionContext, condition, kindOfTriggerCondition);
                        result.Left = CreateExecutors(context, localCodeExecutionContext, bindingVariables, condition.Left, kindOfTriggerCondition, conceptAsTriggerName);
                        result.Right = CreateExecutors(context, localCodeExecutionContext, bindingVariables, condition.Right, kindOfTriggerCondition, conceptAsTriggerName);
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
                                    paramsList.Add(CreateExecutors(context, localCodeExecutionContext, bindingVariables, param, kindOfTriggerCondition, false));
                                }
                            }

                            result.Left = CreateExecutors(context, localCodeExecutionContext, bindingVariables, condition.Left, kindOfTriggerCondition, false);
                        }
                        else
                        {
                            result.Left = CreateExecutors(context, localCodeExecutionContext, bindingVariables, condition.Left, kindOfTriggerCondition, conceptAsTriggerName);
                        }
                        
                        return result;
                    }

                case KindOfTriggerConditionNode.Var:
                    return new VarTriggerConditionNodeExecutor(context.EngineContext, localCodeExecutionContext, condition, kindOfTriggerCondition);

                case KindOfTriggerConditionNode.Value:
                    return new ValueTriggerConditionNodeExecutor(context.EngineContext.Logger, condition, kindOfTriggerCondition);

                case KindOfTriggerConditionNode.Concept:
                    if(conceptAsTriggerName)
                    {
                        return new TriggerNameTriggerConditionNodeExecutor(context.EngineContext, localCodeExecutionContext, condition, kindOfTriggerCondition);
                    }
                    return new ConceptTriggerConditionNodeExecutor(context.EngineContext.Logger, condition);

                case KindOfTriggerConditionNode.Group:
                    {
                        var result = new GroupTriggerConditionNodeExecutor(context.EngineContext.Logger);
                        result.Left = CreateExecutors(context, localCodeExecutionContext, bindingVariables, condition.Left, kindOfTriggerCondition, conceptAsTriggerName);
                        return result;
                    }

                default:
                    throw new ArgumentOutOfRangeException(nameof(condition.Kind), condition.Kind, null);
            }
        }
    }
}
