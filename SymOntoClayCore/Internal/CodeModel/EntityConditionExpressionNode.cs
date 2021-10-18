using NLog;
using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Core.Internal.Convertors;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.Core.Internal.Parsing.Internal.ExprLinking;
using SymOntoClay.CoreHelper.CollectionsHelpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class EntityConditionExpressionNode : AnnotatedItem, IAstNode
    {
#if DEBUG
        private static ILogger _gbcLogger = LogManager.GetCurrentClassLogger();
#endif

        public KindOfLogicalQueryNode Kind { get; set; } = KindOfLogicalQueryNode.Unknown;
        public KindOfOperatorOfLogicalQueryNode KindOfOperator { get; set; } = KindOfOperatorOfLogicalQueryNode.Unknown;
        public StrongIdentifierValue Name { get; set; }
        public EntityConditionExpressionNode Left { get; set; }
        public EntityConditionExpressionNode Right { get; set; }
        public IList<EntityConditionExpressionNode> ParamsList { get; set; }
        public Value Value { get; set; }
        public FuzzyLogicNonNumericSequenceValue FuzzyLogicNonNumericSequenceValue { get; set; }

        /// <inheritdoc/>
        protected override ulong CalculateLongHashCode(CheckDirtyOptions options)
        {
#if DEBUG
            //_gbcLogger.Info($"this = {DebugHelperForRuleInstance.ToString(this)}");
#endif

            switch (Kind)
            {
                case KindOfLogicalQueryNode.BinaryOperator:
                    switch (KindOfOperator)
                    {
                        case KindOfOperatorOfLogicalQueryNode.And:
                        case KindOfOperatorOfLogicalQueryNode.Or:
                        case KindOfOperatorOfLogicalQueryNode.Is:
                        case KindOfOperatorOfLogicalQueryNode.IsNot:
                        case KindOfOperatorOfLogicalQueryNode.More:
                        case KindOfOperatorOfLogicalQueryNode.MoreOrEqual:
                        case KindOfOperatorOfLogicalQueryNode.Less:
                        case KindOfOperatorOfLogicalQueryNode.LessOrEqual:
                            return base.CalculateLongHashCode(options) ^ LongHashCodeWeights.BaseOperatorWeight ^ (ulong)Math.Abs(KindOfOperator.GetHashCode()) ^ Left.GetLongHashCode(options) ^ Right.GetLongHashCode(options);

                        default:
                            throw new ArgumentOutOfRangeException(nameof(KindOfOperator), KindOfOperator, null);
                    }

                case KindOfLogicalQueryNode.UnaryOperator:
                    switch (KindOfOperator)
                    {
                        case KindOfOperatorOfLogicalQueryNode.Not:
                            return base.CalculateLongHashCode(options) ^ LongHashCodeWeights.BaseOperatorWeight ^ (ulong)Math.Abs(KindOfOperator.GetHashCode()) ^ Left.GetLongHashCode(options);

                        default:
                            throw new ArgumentOutOfRangeException(nameof(KindOfOperator), KindOfOperator, null);
                    }

                case KindOfLogicalQueryNode.Concept:
                case KindOfLogicalQueryNode.Entity:
                case KindOfLogicalQueryNode.QuestionVar:
                case KindOfLogicalQueryNode.LogicalVar:
                    return base.CalculateLongHashCode(options) ^ Name.GetLongHashCode(options);

                case KindOfLogicalQueryNode.Value:
                    return base.CalculateLongHashCode(options) ^ Value.GetLongHashCode(options);

                case KindOfLogicalQueryNode.FuzzyLogicNonNumericSequence:
                    return base.CalculateLongHashCode(options) ^ FuzzyLogicNonNumericSequenceValue.GetLongHashCode(options);

                case KindOfLogicalQueryNode.StubParam:
                    return LongHashCodeWeights.StubWeight ^ base.CalculateLongHashCode(options);

                case KindOfLogicalQueryNode.EntityCondition:
                case KindOfLogicalQueryNode.EntityRef:
                    break;

                case KindOfLogicalQueryNode.Relation:
                    {
                        var result = base.CalculateLongHashCode(options) ^ LongHashCodeWeights.BaseFunctionWeight ^ Name.GetLongHashCode(options);

                        foreach (var param in ParamsList)
                        {
                            result ^= LongHashCodeWeights.BaseParamWeight ^ param.GetLongHashCode(options);
                        }

                        return result;
                    }

                case KindOfLogicalQueryNode.Group:
                    return base.CalculateLongHashCode(options) ^ LongHashCodeWeights.GroupWeight ^ Left.GetLongHashCode(options);

                default:
                    throw new ArgumentOutOfRangeException(nameof(Kind), Kind, null);
            }

            throw new NotImplementedException();
        }

        IAstNode IAstNode.Left { get => Left; set => Left = (EntityConditionExpressionNode)value; }
        IAstNode IAstNode.Right { get => Right; set => Right = (EntityConditionExpressionNode)value; }

        /// <inheritdoc/>
        public override AnnotatedItem CloneAnnotatedItem(Dictionary<object, object> context)
        {
            return Clone(context);
        }

        /// <summary>
        /// Clones the instance and returns cloned instance.
        /// </summary>
        /// <returns>Cloned instance.</returns>
        public EntityConditionExpressionNode Clone()
        {
            var context = new Dictionary<object, object>();
            return Clone(context);
        }

        /// <summary>
        /// Clones the instance using special context and returns cloned instance.
        /// </summary>
        /// <param name="context">Special context for providing references continuity.</param>
        /// <returns>Cloned instance.</returns>
        public EntityConditionExpressionNode Clone(Dictionary<object, object> context)
        {
            if (context.ContainsKey(this))
            {
                return (EntityConditionExpressionNode)context[this];
            }

            var result = new EntityConditionExpressionNode();
            context[this] = result;

            result.Kind = Kind;
            result.KindOfOperator = KindOfOperator;
            result.Name = Name?.Clone(context);
            result.Left = Left?.Clone(context);
            result.Right = Right?.Clone(context);
            result.ParamsList = ParamsList?.Select(p => p.Clone(context)).ToList();
            result.Value = Value?.CloneValue(context);
            result.FuzzyLogicNonNumericSequenceValue = FuzzyLogicNonNumericSequenceValue?.Clone(context);
            
            result.AppendAnnotations(this, context);

            return result;
        }

        /// <inheritdoc/>
        public override void DiscoverAllAnnotations(IList<RuleInstance> result)
        {
            base.DiscoverAllAnnotations(result);

            Name?.DiscoverAllAnnotations(result);
            Left?.DiscoverAllAnnotations(result);
            Right?.DiscoverAllAnnotations(result);

            if (!ParamsList.IsNullOrEmpty())
            {
                foreach (var item in ParamsList)
                {
                    item.DiscoverAllAnnotations(result);
                }
            }

            Value?.DiscoverAllAnnotations(result);
            FuzzyLogicNonNumericSequenceValue?.DiscoverAllAnnotations(result);
        }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}{nameof(Kind)} = {Kind}");
            sb.AppendLine($"{spaces}{nameof(KindOfOperator)} = {KindOfOperator}");

            sb.PrintObjProp(n, nameof(Name), Name);

            sb.PrintObjProp(n, nameof(Left), Left);
            sb.PrintObjProp(n, nameof(Right), Right);
            sb.PrintObjListProp(n, nameof(ParamsList), ParamsList);

            sb.PrintObjProp(n, nameof(Value), Value);
            sb.PrintObjProp(n, nameof(FuzzyLogicNonNumericSequenceValue), FuzzyLogicNonNumericSequenceValue);

            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}{nameof(Kind)} = {Kind}");
            sb.AppendLine($"{spaces}{nameof(KindOfOperator)} = {KindOfOperator}");

            sb.PrintShortObjProp(n, nameof(Name), Name);

            sb.PrintShortObjProp(n, nameof(Left), Left);
            sb.PrintShortObjProp(n, nameof(Right), Right);
            sb.PrintShortObjListProp(n, nameof(ParamsList), ParamsList);

            sb.PrintShortObjProp(n, nameof(Value), Value);
            sb.PrintShortObjProp(n, nameof(FuzzyLogicNonNumericSequenceValue), FuzzyLogicNonNumericSequenceValue);

            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}{nameof(Kind)} = {Kind}");
            sb.AppendLine($"{spaces}{nameof(KindOfOperator)} = {KindOfOperator}");

            sb.PrintBriefObjProp(n, nameof(Name), Name);

            sb.PrintExisting(n, nameof(Left), Left);
            sb.PrintExisting(n, nameof(Right), Right);
            sb.PrintExistingList(n, nameof(ParamsList), ParamsList);

            sb.PrintBriefObjProp(n, nameof(Value), Value);
            sb.PrintBriefObjProp(n, nameof(FuzzyLogicNonNumericSequenceValue), FuzzyLogicNonNumericSequenceValue);

            sb.Append(base.PropertiesToBriefString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToDbgString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            return $"{spaces}{GetHumanizeDbgString()}";
        }

        public string GetHumanizeDbgString()
        {
            return DebugHelperForEntityConditionExpression.ToString(this);
        }
    }
}
