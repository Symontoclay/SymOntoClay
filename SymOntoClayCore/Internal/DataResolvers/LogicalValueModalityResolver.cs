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
using SymOntoClay.CoreHelper;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.DataResolvers
{
    public class LogicalValueModalityResolver : BaseResolver
    {
        public LogicalValueModalityResolver(IMainStorageContext context)
            : base(context)
        {
            var dataResolversFactory = context.DataResolversFactory;

            _fuzzyLogicResolver = dataResolversFactory.GetFuzzyLogicResolver();
            _toSystemBoolResolver = dataResolversFactory.GetToSystemBoolResolver();
        }

        private readonly FuzzyLogicResolver _fuzzyLogicResolver;
        private readonly ToSystemBoolResolver _toSystemBoolResolver;

        public bool IsHigh(Value modalityValue, ILocalCodeExecutionContext localCodeExecutionContext)
        {
#if DEBUG
            //Log($"modalityValue = {modalityValue}");
#endif

            if(modalityValue == null)
            {
                return false;
            }

            if(modalityValue.KindOfValue == KindOfValue.NullValue)
            {
                return false;
            }

            var numberValue = _fuzzyLogicResolver.Resolve(modalityValue, localCodeExecutionContext);

#if DEBUG
            //Log($"numberValue = {numberValue}");
#endif

            return _toSystemBoolResolver.Resolve(numberValue);
        }

        public bool IsFit(Value modalityValue, Value queryModalityValue, ILocalCodeExecutionContext localCodeExecutionContext)
        {
#if DEBUG
            //Log($"modalityValue = {modalityValue}");
            //Log($"queryModalityValue = {queryModalityValue}");
#endif

            if(modalityValue == null)
            {
                return false;
            }

            if(modalityValue.KindOfValue == KindOfValue.NullValue)
            {
                return false;
            }

            if(queryModalityValue.IsLogicalModalityExpressionValue)
            {
                var exprValue = queryModalityValue.AsLogicalModalityExpressionValue;

#if DEBUG
                //Log($"exprValue.Expression = {exprValue.Expression.ToHumanizedString()}");
#endif

                return ProcessBoolExpression(modalityValue, exprValue.Expression, localCodeExecutionContext);
            }

            return _fuzzyLogicResolver.Equals(modalityValue, queryModalityValue, localCodeExecutionContext);
        }

        private bool ProcessBoolExpression(Value modalityValue, LogicalModalityExpressionNode expressionNode, ILocalCodeExecutionContext localCodeExecutionContext)
        {
#if DEBUG
            //Log($"exprValue.Expression = {expressionNode.ToHumanizedString()}");
#endif

            switch (expressionNode.Kind)
            {
                case KindOfLogicalModalityExpressionNode.BinaryOperator:
                    return ProcessBinaryOperator(modalityValue, expressionNode, localCodeExecutionContext);

                case KindOfLogicalModalityExpressionNode.UnaryOperator:
                    return ProcessUnaryOperator(modalityValue, expressionNode, localCodeExecutionContext);

                case KindOfLogicalModalityExpressionNode.Group:
                    return ProcessGroup(modalityValue, expressionNode, localCodeExecutionContext);

                default:
                    throw new ArgumentOutOfRangeException(nameof(expressionNode.Kind), expressionNode.Kind, null);
            }
        }

        private Value ProcessValueExpression(Value modalityValue, LogicalModalityExpressionNode expressionNode, ILocalCodeExecutionContext localCodeExecutionContext)
        {
#if DEBUG
            //Log($"exprValue.Expression = {expressionNode.ToHumanizedString()}");
#endif

            switch (expressionNode.Kind)
            {
                case KindOfLogicalModalityExpressionNode.BlankIdentifier:
                    return ProcessBlankIdentifier(modalityValue, expressionNode, localCodeExecutionContext);

                case KindOfLogicalModalityExpressionNode.Value:
                    return ProcessValue(modalityValue, expressionNode, localCodeExecutionContext);

                default:
                    throw new ArgumentOutOfRangeException(nameof(expressionNode.Kind), expressionNode.Kind, null);
            }
        }

        private Value ProcessBlankIdentifier(Value modalityValue, LogicalModalityExpressionNode expressionNode, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return modalityValue;
        }

        private bool ProcessBinaryOperator(Value modalityValue, LogicalModalityExpressionNode expressionNode, ILocalCodeExecutionContext localCodeExecutionContext)
        {
#if DEBUG
            //Log($"expressionNode = {expressionNode.ToHumanizedString()}");
#endif

            switch (expressionNode.KindOfOperator)
            {
                case KindOfOperator.Is:
                    {
                        var left = ProcessValueExpression(modalityValue, expressionNode.Left, localCodeExecutionContext);

#if DEBUG
                        //Log($"left = {left}");
#endif

                        var right = ProcessValueExpression(modalityValue, expressionNode.Right, localCodeExecutionContext);

#if DEBUG
                        //Log($"right = {right}");
#endif

                        return _fuzzyLogicResolver.Equals(left, right, localCodeExecutionContext);
                    }

                case KindOfOperator.IsNot:
                    {
                        var left = ProcessValueExpression(modalityValue, expressionNode.Left, localCodeExecutionContext);

#if DEBUG
                        //Log($"left = {left}");
#endif

                        var right = ProcessValueExpression(modalityValue, expressionNode.Right, localCodeExecutionContext);

#if DEBUG
                        //Log($"right = {right}");
#endif

                        return !_fuzzyLogicResolver.Equals(left, right, localCodeExecutionContext);
                    }

                case KindOfOperator.And:
                    {
                        var left = ProcessBoolExpression(modalityValue, expressionNode.Left, localCodeExecutionContext);

#if DEBUG
                        //Log($"left = {left}");
#endif

                        var right = ProcessBoolExpression(modalityValue, expressionNode.Right, localCodeExecutionContext);

#if DEBUG
                        //Log($"right = {right}");
#endif

                        return left && right;
                    }

                case KindOfOperator.Or:
                    {
                        var left = ProcessBoolExpression(modalityValue, expressionNode.Left, localCodeExecutionContext);

#if DEBUG
                        //Log($"left = {left}");
#endif

                        var right = ProcessBoolExpression(modalityValue, expressionNode.Right, localCodeExecutionContext);

#if DEBUG
                        //Log($"right = {right}");
#endif

                        return left || right;
                    }

                case KindOfOperator.More:
                    {
                        var left = ProcessValueExpression(modalityValue, expressionNode.Left, localCodeExecutionContext);

#if DEBUG
                        //Log($"left = {left}");
#endif

                        var right = ProcessValueExpression(modalityValue, expressionNode.Right, localCodeExecutionContext);

#if DEBUG
                        //Log($"right = {right}");
#endif

                        return _fuzzyLogicResolver.More(left, right, localCodeExecutionContext);
                    }

                case KindOfOperator.MoreOrEqual:
                    {
                        var left = ProcessValueExpression(modalityValue, expressionNode.Left, localCodeExecutionContext);

#if DEBUG
                        //Log($"left = {left}");
#endif

                        var right = ProcessValueExpression(modalityValue, expressionNode.Right, localCodeExecutionContext);

#if DEBUG
                        //Log($"right = {right}");
#endif

                        return _fuzzyLogicResolver.MoreOrEqual(left, right, localCodeExecutionContext);
                    }

                case KindOfOperator.Less:
                    {
                        var left = ProcessValueExpression(modalityValue, expressionNode.Left, localCodeExecutionContext);

#if DEBUG
                        //Log($"left = {left}");
#endif

                        var right = ProcessValueExpression(modalityValue, expressionNode.Right, localCodeExecutionContext);

#if DEBUG
                        //Log($"right = {right}");
#endif

                        return _fuzzyLogicResolver.Less(left, right, localCodeExecutionContext);
                    }

                case KindOfOperator.LessOrEqual:
                    {
                        var left = ProcessValueExpression(modalityValue, expressionNode.Left, localCodeExecutionContext);

#if DEBUG
                        //Log($"left = {left}");
#endif

                        var right = ProcessValueExpression(modalityValue, expressionNode.Right, localCodeExecutionContext);

#if DEBUG
                        //Log($"right = {right}");
#endif

                        return _fuzzyLogicResolver.LessOrEqual(left, right, localCodeExecutionContext);
                    }

                default:
                    throw new ArgumentOutOfRangeException(nameof(expressionNode.KindOfOperator), expressionNode.KindOfOperator, null);
            }
        }

        private bool ProcessUnaryOperator(Value modalityValue, LogicalModalityExpressionNode expressionNode, ILocalCodeExecutionContext localCodeExecutionContext)
        {
#if DEBUG
            //Log($"expressionNode = {expressionNode.ToHumanizedString()}");
#endif

            switch (expressionNode.KindOfOperator)
            {
                case KindOfOperator.Not:
                    return !ProcessBoolExpression(modalityValue, expressionNode.Left, localCodeExecutionContext);

                default:
                    throw new ArgumentOutOfRangeException(nameof(expressionNode.KindOfOperator), expressionNode.KindOfOperator, null);
            }
        }

        private Value ProcessValue(Value modalityValue, LogicalModalityExpressionNode expressionNode, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return expressionNode.Value;
        }

        private bool ProcessGroup(Value modalityValue, LogicalModalityExpressionNode expressionNode, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return ProcessBoolExpression(modalityValue, expressionNode.Left, localCodeExecutionContext);
        }
    }
}
