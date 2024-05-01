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
using SymOntoClay.Common.DebugHelpers;
using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.Core.Internal.Parsing.Internal.ExprLinking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class EntityConditionExpressionNode : AnnotatedItem, IAstNode
    {
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

        /// <include file = "..\CommonDoc.xml" path='extradoc/method[@name="Clone"]/*' />
        public EntityConditionExpressionNode Clone()
        {
            var context = new Dictionary<object, object>();
            return Clone(context);
        }

        /// <include file = "..\CommonDoc.xml" path='extradoc/method[@name="CloneWithContext"]/*' />
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
        public override void DiscoverAllAnnotations(IList<Annotation> result)
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
            return $"{spaces}{ToHumanizedString()}";
        }

        /// <inheritdoc/>
        public override string ToHumanizedString(DebugHelperOptions options)
        {
            return DebugHelperForEntityConditionExpression.ToString(this, options);
        }

        /// <inheritdoc/>
        public override string ToHumanizedLabel(DebugHelperOptions options)
        {
            return ToHumanizedString(options);
        }
    }
}
