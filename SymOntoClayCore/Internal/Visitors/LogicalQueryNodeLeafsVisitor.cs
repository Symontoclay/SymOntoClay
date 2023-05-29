using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.CoreHelper.CollectionsHelpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Visitors
{
    public class LogicalQueryNodeLeafsVisitor : BaseLoggedComponent, ILogicalVisitor
    {
        public LogicalQueryNodeLeafsVisitor()
            : this(LoggerNLogImpementation.Instance)
        {
        }

        public LogicalQueryNodeLeafsVisitor(IEntityLogger logger)
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
#if DEBUG
            Log($"logicalQueryNode.Kind = {logicalQueryNode.Kind}");
#endif

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
