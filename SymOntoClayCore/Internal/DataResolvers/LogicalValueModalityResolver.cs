/*MIT License

Copyright (c) 2020 - 2026 Sergiy Tolkachov

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

using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Ast.Expressions;
using SymOntoClay.Monitor.Common;
using System;

namespace SymOntoClay.Core.Internal.DataResolvers
{
    public class LogicalValueModalityResolver : BaseResolver
    {
        public LogicalValueModalityResolver(IMainStorageContext context)
            : base(context)
        {
        }

        /// <inheritdoc/>
        protected override void LinkWithOtherBaseContextComponents()
        {
            base.LinkWithOtherBaseContextComponents();

            var dataResolversFactory = _context.DataResolversFactory;

            _fuzzyLogicResolver = dataResolversFactory.GetFuzzyLogicResolver();
            _toSystemBoolResolver = dataResolversFactory.GetToSystemBoolResolver();
        }

        private FuzzyLogicResolver _fuzzyLogicResolver;
        private ToSystemBoolResolver _toSystemBoolResolver;

        public bool IsHigh(IMonitorLogger logger, Value modalityValue, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            if(modalityValue == null)
            {
                return false;
            }

            if(modalityValue.KindOfValue == KindOfValue.NullValue)
            {
                return false;
            }

            var numberValue = _fuzzyLogicResolver.Resolve(logger, modalityValue, localCodeExecutionContext);

            return _toSystemBoolResolver.Resolve(logger, numberValue);
        }

        public bool IsFit(IMonitorLogger logger, Value modalityValue, Value queryModalityValue, ILocalCodeExecutionContext localCodeExecutionContext)
        {
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

                return ProcessBoolExpression(logger, modalityValue, exprValue.Expression, localCodeExecutionContext);
            }

            return _fuzzyLogicResolver.Equals(logger, modalityValue, queryModalityValue, localCodeExecutionContext);
        }

        private bool ProcessBoolExpression(IMonitorLogger logger, Value modalityValue, LogicalModalityExpressionNode expressionNode, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            switch (expressionNode.Kind)
            {
                case KindOfLogicalModalityExpressionNode.BinaryOperator:
                    return ProcessBinaryOperator(logger, modalityValue, expressionNode, localCodeExecutionContext);

                case KindOfLogicalModalityExpressionNode.UnaryOperator:
                    return ProcessUnaryOperator(logger, modalityValue, expressionNode, localCodeExecutionContext);

                case KindOfLogicalModalityExpressionNode.Group:
                    return ProcessGroup(logger, modalityValue, expressionNode, localCodeExecutionContext);

                default:
                    throw new ArgumentOutOfRangeException(nameof(expressionNode.Kind), expressionNode.Kind, "405BB8FF-7004-420B-9C88-A69A40B204CD");
            }
        }

        private Value ProcessValueExpression(IMonitorLogger logger, Value modalityValue, LogicalModalityExpressionNode expressionNode, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            switch (expressionNode.Kind)
            {
                case KindOfLogicalModalityExpressionNode.BlankIdentifier:
                    return ProcessBlankIdentifier(logger, modalityValue, expressionNode, localCodeExecutionContext);

                case KindOfLogicalModalityExpressionNode.Value:
                    return ProcessValue(logger, modalityValue, expressionNode, localCodeExecutionContext);

                default:
                    throw new ArgumentOutOfRangeException(nameof(expressionNode.Kind), expressionNode.Kind, "99B0B4ED-E6BE-468E-A10C-D62DE2704119");
            }
        }

        private Value ProcessBlankIdentifier(IMonitorLogger logger, Value modalityValue, LogicalModalityExpressionNode expressionNode, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return modalityValue;
        }

        private bool ProcessBinaryOperator(IMonitorLogger logger, Value modalityValue, LogicalModalityExpressionNode expressionNode, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            switch (expressionNode.KindOfOperator)
            {
                case KindOfOperator.Is:
                    {
                        var left = ProcessValueExpression(logger, modalityValue, expressionNode.Left, localCodeExecutionContext);

                        var right = ProcessValueExpression(logger, modalityValue, expressionNode.Right, localCodeExecutionContext);

                        return _fuzzyLogicResolver.Equals(logger, left, right, localCodeExecutionContext);
                    }

                case KindOfOperator.IsNot:
                    {
                        var left = ProcessValueExpression(logger, modalityValue, expressionNode.Left, localCodeExecutionContext);

                        var right = ProcessValueExpression(logger, modalityValue, expressionNode.Right, localCodeExecutionContext);

                        return !_fuzzyLogicResolver.Equals(logger, left, right, localCodeExecutionContext);
                    }

                case KindOfOperator.And:
                    {
                        var left = ProcessBoolExpression(logger, modalityValue, expressionNode.Left, localCodeExecutionContext);

                        var right = ProcessBoolExpression(logger, modalityValue, expressionNode.Right, localCodeExecutionContext);

                        return left && right;
                    }

                case KindOfOperator.Or:
                    {
                        var left = ProcessBoolExpression(logger, modalityValue, expressionNode.Left, localCodeExecutionContext);

                        var right = ProcessBoolExpression(logger, modalityValue, expressionNode.Right, localCodeExecutionContext);

                        return left || right;
                    }

                case KindOfOperator.More:
                    {
                        var left = ProcessValueExpression(logger, modalityValue, expressionNode.Left, localCodeExecutionContext);

                        var right = ProcessValueExpression(logger, modalityValue, expressionNode.Right, localCodeExecutionContext);

                        return _fuzzyLogicResolver.More(logger, left, right, localCodeExecutionContext);
                    }

                case KindOfOperator.MoreOrEqual:
                    {
                        var left = ProcessValueExpression(logger, modalityValue, expressionNode.Left, localCodeExecutionContext);

                        var right = ProcessValueExpression(logger, modalityValue, expressionNode.Right, localCodeExecutionContext);

                        return _fuzzyLogicResolver.MoreOrEqual(logger, left, right, localCodeExecutionContext);
                    }

                case KindOfOperator.Less:
                    {
                        var left = ProcessValueExpression(logger, modalityValue, expressionNode.Left, localCodeExecutionContext);

                        var right = ProcessValueExpression(logger, modalityValue, expressionNode.Right, localCodeExecutionContext);

                        return _fuzzyLogicResolver.Less(logger, left, right, localCodeExecutionContext);
                    }

                case KindOfOperator.LessOrEqual:
                    {
                        var left = ProcessValueExpression(logger, modalityValue, expressionNode.Left, localCodeExecutionContext);

                        var right = ProcessValueExpression(logger, modalityValue, expressionNode.Right, localCodeExecutionContext);

                        return _fuzzyLogicResolver.LessOrEqual(logger, left, right, localCodeExecutionContext);
                    }

                default:
                    throw new ArgumentOutOfRangeException(nameof(expressionNode.KindOfOperator), expressionNode.KindOfOperator, "5F2ECA38-0A26-4D35-A29A-E49AE895363E");
            }
        }

        private bool ProcessUnaryOperator(IMonitorLogger logger, Value modalityValue, LogicalModalityExpressionNode expressionNode, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            switch (expressionNode.KindOfOperator)
            {
                case KindOfOperator.Not:
                    return !ProcessBoolExpression(logger, modalityValue, expressionNode.Left, localCodeExecutionContext);

                default:
                    throw new ArgumentOutOfRangeException(nameof(expressionNode.KindOfOperator), expressionNode.KindOfOperator, "799463B9-2483-4696-BE5E-855516B245BE");
            }
        }

        private Value ProcessValue(IMonitorLogger logger, Value modalityValue, LogicalModalityExpressionNode expressionNode, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return expressionNode.Value;
        }

        private bool ProcessGroup(IMonitorLogger logger, Value modalityValue, LogicalModalityExpressionNode expressionNode, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return ProcessBoolExpression(logger, modalityValue, expressionNode.Left, localCodeExecutionContext);
        }
    }
}
