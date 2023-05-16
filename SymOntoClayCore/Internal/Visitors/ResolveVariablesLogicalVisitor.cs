using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Ast.Expressions;
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.CoreHelper.CollectionsHelpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Xml.Linq;

namespace SymOntoClay.Core.Internal.Visitors
{
    public class ResolveVariablesLogicalVisitor : BaseLoggedComponent, ILogicalVisitor
    {
        public ResolveVariablesLogicalVisitor(IEntityLogger logger)
            : base(logger)
        {
        }

        public void Run(RuleInstance ruleInstance, IPackedVarsResolver varsResolver)
        {
            _varsResolver = varsResolver;
            ruleInstance.Accept(this);
        }

        private IPackedVarsResolver _varsResolver;

        /// <inheritdoc/>
        public void VisitRuleInstance(RuleInstance ruleInstance)
        {
            var primaryPart = ruleInstance.PrimaryPart;
            var secondaryParts = ruleInstance.SecondaryParts;

            if (primaryPart != null)
            {
                if (primaryPart.IsParameterized)
                {
                    primaryPart.Accept(this);
                }
            }

            if (!secondaryParts.IsNullOrEmpty())
            {
                foreach (var item in secondaryParts)
                {
                    if (item.IsParameterized)
                    {
                        item.Accept(this);
                    }
                }
            }

            ruleInstance.IsParameterized = false;
        }

        /// <inheritdoc/>
        public void VisitPrimaryRulePart(PrimaryRulePart primaryRulePart)
        {
            primaryRulePart.Expression.Accept(this);

            primaryRulePart.IsParameterized = false;
        }

        /// <inheritdoc/>
        public void VisitSecondaryRulePart(SecondaryRulePart secondaryRulePart)
        {
            secondaryRulePart.Expression.Accept(this);

            secondaryRulePart.IsParameterized = false;
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

                case KindOfLogicalQueryNode.Value:
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

                case KindOfLogicalQueryNode.Var:
                    {
#if DEBUG
                        //Log($"Name = {logicalQueryNode.Name}");
#endif

                        var value = _varsResolver.GetVarValue(logicalQueryNode.Name);

#if DEBUG
                        //Log($"value = {value.ToHumanizedString()}");
#endif

                        if (value.IsStrongIdentifierValue)
                        {
                            var strVal = value.AsStrongIdentifierValue;

                            var kindOfName = strVal.KindOfName;

                            switch (kindOfName)
                            {
                                case KindOfName.Concept:
                                    logicalQueryNode.Kind = KindOfLogicalQueryNode.Concept;
                                    logicalQueryNode.Name = strVal;
                                    break;

                                case KindOfName.Entity:
                                    logicalQueryNode.Kind = KindOfLogicalQueryNode.Entity;
                                    logicalQueryNode.Name = strVal;
                                    break;

                                default:
                                    throw new ArgumentOutOfRangeException(nameof(kindOfName), kindOfName, null);
                            }
                        }
                        else
                        {
                            logicalQueryNode.Kind = KindOfLogicalQueryNode.Value;
                            logicalQueryNode.Name = null;
                            logicalQueryNode.Value = value;
                        }
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kind), kind, null);
            }
        }
    }
}
