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

                case KindOfTriggerConditionNode.Each:
                    result.Add(new EachTriggerConditionNodeObserver(context, condition));
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
                    result.Add(new TriggerNameTriggerConditionNodeObserver(context.EngineContext, context.Storage, condition));
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(condition.Kind), condition.Kind, null);
            }
        }
    }
}
