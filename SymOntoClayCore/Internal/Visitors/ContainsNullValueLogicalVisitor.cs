using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Ast.Expressions;
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.CoreHelper.CollectionsHelpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Visitors
{
    public class ContainsNullValueLogicalVisitor: BaseLoggedComponent, ILogicalVisitor
    {
        public ContainsNullValueLogicalVisitor(IEntityLogger logger)
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
