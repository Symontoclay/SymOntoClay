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

using NLog;
using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.CodeModel.Ast.Expressions;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.Core.Internal.Parsing.Internal.ExprLinking;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel.ConditionOfTriggerExpr
{
    public class TriggerConditionNode : AnnotatedItem, IAstNode
    {
        public KindOfTriggerConditionNode Kind { get; set; } = KindOfTriggerConditionNode.Unknown;
        public KindOfOperator KindOfOperator { get; set; } = KindOfOperator.Unknown;
        public StrongIdentifierValue Name { get; set; }
        public TriggerConditionNode Left { get; set; }
        public TriggerConditionNode Right { get; set; }
        public bool IsNamedParameters { get; set; }
        public IList<TriggerConditionNode> ParamsList { get; set; }
        public RuleInstance RuleInstance { get; set; }
        public Value Value { get; set; }
        public FuzzyLogicNonNumericSequenceValue FuzzyLogicNonNumericSequenceValue { get; set; }

        /// <inheritdoc/>
        protected override ulong CalculateLongHashCode(CheckDirtyOptions options)
        {
            switch (Kind)
            {
                case KindOfTriggerConditionNode.BinaryOperator:
                    return base.CalculateLongHashCode(options) ^ LongHashCodeWeights.BaseOperatorWeight ^ (ulong)Math.Abs(KindOfOperator.GetHashCode()) ^ Left.GetLongHashCode(options) ^ Right.GetLongHashCode(options);

                case KindOfTriggerConditionNode.UnaryOperator:
                    return base.CalculateLongHashCode(options) ^ LongHashCodeWeights.BaseOperatorWeight ^ (ulong)Math.Abs(KindOfOperator.GetHashCode()) ^ Left.GetLongHashCode(options);

                case KindOfTriggerConditionNode.Concept:
                case KindOfTriggerConditionNode.Entity:
                    return base.CalculateLongHashCode(options) ^ Name.GetLongHashCode(options);

                case KindOfTriggerConditionNode.Value:
                    return base.CalculateLongHashCode(options) ^ Value.GetLongHashCode(options);

                case KindOfTriggerConditionNode.FuzzyLogicNonNumericSequence:
                    return base.CalculateLongHashCode(options) ^ FuzzyLogicNonNumericSequenceValue.GetLongHashCode(options);

                case KindOfTriggerConditionNode.EntityCondition:
                case KindOfTriggerConditionNode.EntityRef:
                    break;

                case KindOfTriggerConditionNode.Var:
                    return 0;

                case KindOfTriggerConditionNode.Fact:
                    return base.CalculateLongHashCode(options) ^ RuleInstance.GetLongHashCode(options);

                case KindOfTriggerConditionNode.Duration:
                case KindOfTriggerConditionNode.Each:
                case KindOfTriggerConditionNode.Once:
                    return base.CalculateLongHashCode(options) ^ LongHashCodeWeights.StubWeight ^ Value.GetLongHashCode(options);

                case KindOfTriggerConditionNode.Group:
                    return base.CalculateLongHashCode(options) ^ LongHashCodeWeights.GroupWeight ^ Left.GetLongHashCode(options);
                    
                default:
                    throw new ArgumentOutOfRangeException(nameof(Kind), Kind, null);
            }

            throw new NotImplementedException();
        }

        IAstNode IAstNode.Left { get => Left; set => Left = (TriggerConditionNode)value; }
        IAstNode IAstNode.Right { get => Right; set => Right = (TriggerConditionNode)value; }

        /// <inheritdoc/>
        public override AnnotatedItem CloneAnnotatedItem(Dictionary<object, object> context)
        {
            return Clone(context);
        }

        /// <include file = "..\CommonDoc.xml" path='extradoc/method[@name="Clone"]/*' />
        public TriggerConditionNode Clone()
        {
            var context = new Dictionary<object, object>();
            return Clone(context);
        }

        /// <include file = "..\CommonDoc.xml" path='extradoc/method[@name="CloneWithContext"]/*' />
        public TriggerConditionNode Clone(Dictionary<object, object> context)
        {
            if (context.ContainsKey(this))
            {
                return (TriggerConditionNode)context[this];
            }

            var result = new TriggerConditionNode();
            context[this] = result;

            result.Kind = Kind;
            result.KindOfOperator = KindOfOperator;
            result.Name = Name?.Clone(context);
            result.Left = Left?.Clone(context);
            result.Right = Right?.Clone(context);
            result.IsNamedParameters = IsNamedParameters;
            result.ParamsList = ParamsList?.Select(p => p.Clone(context)).ToList();
            result.RuleInstance = RuleInstance?.Clone(context);
            result.Value = Value?.CloneValue(context);
            result.FuzzyLogicNonNumericSequenceValue = FuzzyLogicNonNumericSequenceValue?.Clone(context);

            result.AppendAnnotations(this, context);

            return result;
        }

        /// <inheritdoc/>
        public override void DiscoverAllAnnotations(IList<Annotation> result)
        {
            base.DiscoverAllAnnotations(result);

            throw new NotImplementedException();
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

            sb.AppendLine($"{spaces}{nameof(IsNamedParameters)} = {IsNamedParameters}");

            sb.PrintObjListProp(n, nameof(ParamsList), ParamsList);

            sb.PrintObjProp(n, nameof(RuleInstance), RuleInstance);

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

            sb.AppendLine($"{spaces}{nameof(IsNamedParameters)} = {IsNamedParameters}");

            sb.PrintShortObjListProp(n, nameof(ParamsList), ParamsList);

            sb.PrintShortObjProp(n, nameof(RuleInstance), RuleInstance);

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

            sb.AppendLine($"{spaces}{nameof(IsNamedParameters)} = {IsNamedParameters}");

            sb.PrintExistingList(n, nameof(ParamsList), ParamsList);

            sb.PrintBriefObjProp(n, nameof(RuleInstance), RuleInstance);

            sb.PrintBriefObjProp(n, nameof(Value), Value);
            sb.PrintBriefObjProp(n, nameof(FuzzyLogicNonNumericSequenceValue), FuzzyLogicNonNumericSequenceValue);

            sb.Append(base.PropertiesToBriefString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        public override string ToHumanizedString(DebugHelperOptions options)
        {
            return DebugHelperForTriggerCondition.ToString(this);
        }
    }
}
