/*MIT License

Copyright (c) 2020 - 2024 Sergiy Tolkachov

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

using SymOntoClay.Common.CollectionsHelpers;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Monitor.NLog;
using System;
using System.Collections.Generic;

namespace SymOntoClay.Core.Internal.Visitors
{
    public class LogicalQueryNodeLeavesVisitor : BaseLoggedComponent, ILogicalVisitor
    {
        public LogicalQueryNodeLeavesVisitor()
            : this(MonitorLoggerNLogImplementation.Instance)
        {
        }

        public LogicalQueryNodeLeavesVisitor(IMonitorLogger logger)
            : base(logger)
        {
        }

        public List<LogicalQueryNode> Run(RuleInstance ruleInstance)
        {
            ruleInstance.Accept(this);
            return _leafsList;
        }

        private List<LogicalQueryNode> _leafsList = new List<LogicalQueryNode>();

        /// <inheritdoc/>
        public void VisitRuleInstance(RuleInstance ruleInstance)
        {
            var primaryPart = ruleInstance.PrimaryPart;

            if (primaryPart != null)
            {
                primaryPart.Accept(this);
            }

            var secondaryParts = ruleInstance.SecondaryParts;

            if (!secondaryParts.IsNullOrEmpty())
            {
                foreach (var item in secondaryParts)
                {
                    item.Accept(this);
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

                case KindOfLogicalQueryNode.Relation:
                    foreach (var param in logicalQueryNode.ParamsList)
                    {
                        param.Accept(this);
                    }
                    break;

                case KindOfLogicalQueryNode.Group:
                    left.Accept(this);
                    break;

                case KindOfLogicalQueryNode.Fact:
                    logicalQueryNode.Fact.Accept(this);
                    break;

                case KindOfLogicalQueryNode.Concept:
                case KindOfLogicalQueryNode.Entity:
                case KindOfLogicalQueryNode.Value:
                    _leafsList.Add(logicalQueryNode);
                    break;

                case KindOfLogicalQueryNode.QuestionVar:
                case KindOfLogicalQueryNode.LogicalVar:
                case KindOfLogicalQueryNode.Var:
                case KindOfLogicalQueryNode.StubParam:
                case KindOfLogicalQueryNode.EntityCondition:
                case KindOfLogicalQueryNode.EntityRef:
                case KindOfLogicalQueryNode.FuzzyLogicNonNumericSequence:
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kind), kind, null);
            }
        }
    }
}
