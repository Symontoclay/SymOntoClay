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

using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Ast.Expressions;
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.CoreHelper.CollectionsHelpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.Monitor.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Visitors
{
    public class ContainsNullValueLogicalVisitor: BaseLoggedComponent, ILogicalVisitor
    {
        public ContainsNullValueLogicalVisitor(IMonitorLogger logger)
            : base(logger)
        {
        }

        public bool Run(RuleInstance ruleInstance)
        {
            ruleInstance.Accept(this);
            return _hasNull;
        }

        private bool _hasNull;

        /// <inheritdoc/>
        public void VisitRuleInstance(RuleInstance ruleInstance)
        {
            var primaryPart = ruleInstance.PrimaryPart;

            if (primaryPart != null)
            {
                primaryPart.Accept(this);
            }

            if(_hasNull)
            {
                return;
            }

            var secondaryParts = ruleInstance.SecondaryParts;

            if (!secondaryParts.IsNullOrEmpty())
            {
                foreach (var item in secondaryParts)
                {
                    item.Accept(this);

                    if (_hasNull)
                    {
                        return;
                    }
                }
            }
        }

        /// <inheritdoc/>
        public void VisitPrimaryRulePart(PrimaryRulePart primaryRulePart)
        {
            primaryRulePart.Expression.Accept(this);
        }

        /// <inheritdoc/>
        public void VisitSecondaryRulePart(SecondaryRulePart secondaryRulePart)
        {
            secondaryRulePart.Expression.Accept(this);
        }

        /// <inheritdoc/>
        public void VisitLogicalQueryNode(LogicalQueryNode logicalQueryNode)
        {
            var kind = logicalQueryNode.Kind;
            var kindOfOperator = logicalQueryNode.KindOfOperator;
            var left = logicalQueryNode.Left;
            var right = logicalQueryNode.Right;

            switch (kind)
            {
                case KindOfLogicalQueryNode.BinaryOperator:
                    switch (kindOfOperator)
                    {
                        case KindOfOperatorOfLogicalQueryNode.And:
                        case KindOfOperatorOfLogicalQueryNode.Or:
                        case KindOfOperatorOfLogicalQueryNode.Is:
                        case KindOfOperatorOfLogicalQueryNode.IsNot:
                        case KindOfOperatorOfLogicalQueryNode.More:
                        case KindOfOperatorOfLogicalQueryNode.MoreOrEqual:
                        case KindOfOperatorOfLogicalQueryNode.Less:
                        case KindOfOperatorOfLogicalQueryNode.LessOrEqual:
                            left.Accept(this);
                            right.Accept(this);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(kindOfOperator), kindOfOperator, null);
                    }
                    break;

                case KindOfLogicalQueryNode.UnaryOperator:
                    switch (kindOfOperator)
                    {
                        case KindOfOperatorOfLogicalQueryNode.Not:
                            left.Accept(this);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(kindOfOperator), kindOfOperator, null);
                    }
                    break;

                case KindOfLogicalQueryNode.Concept:
                case KindOfLogicalQueryNode.Entity:
                case KindOfLogicalQueryNode.QuestionVar:
                    break;

                case KindOfLogicalQueryNode.LogicalVar:
                    break;

                case KindOfLogicalQueryNode.StubParam:
                case KindOfLogicalQueryNode.EntityCondition:
                case KindOfLogicalQueryNode.EntityRef:
                case KindOfLogicalQueryNode.Fact:
                    break;

                case KindOfLogicalQueryNode.FuzzyLogicNonNumericSequence:
                    break;

                case KindOfLogicalQueryNode.Relation:
                    foreach (var param in logicalQueryNode.ParamsList)
                    {
                        param.Accept(this);
                    }
                    break;

                case KindOfLogicalQueryNode.Group:
                    left.Accept(this);
                    break;

                case KindOfLogicalQueryNode.Value:
                    {
                        if(logicalQueryNode.Value.IsNullValue)
                        {
                            _hasNull = true;
                            return;
                        }
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kind), kind, null);
            }
        }
    }
}
