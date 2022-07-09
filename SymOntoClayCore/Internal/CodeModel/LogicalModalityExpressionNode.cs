﻿using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.CodeModel.Ast.Expressions;
using SymOntoClay.Core.Internal.Parsing.Internal.ExprLinking;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class LogicalModalityExpressionNode : AnnotatedItem, IAstNode
    {
        public KindOfLogicalModalityExpressionNode Kind { get; set; } = KindOfLogicalModalityExpressionNode.Unknown;
        public KindOfOperator KindOfOperator { get; set; } = KindOfOperator.Unknown;

        public LogicalModalityExpressionNode Left { get; set; }
        public LogicalModalityExpressionNode Right { get; set; }

        public Value Value { get; set; }

        /// <inheritdoc/>
        IAstNode IAstNode.Left { get => Left; set => Left = (LogicalModalityExpressionNode)value; }

        /// <inheritdoc/>
        IAstNode IAstNode.Right { get => Right; set => Right = (LogicalModalityExpressionNode)value; }

        /// <inheritdoc/>
        public override AnnotatedItem CloneAnnotatedItem(Dictionary<object, object> context)
        {
            return Clone(context);
        }

        /// <summary>
        /// Clones the instance and returns cloned instance.
        /// </summary>
        /// <returns>Cloned instance.</returns>
        public LogicalModalityExpressionNode Clone()
        {
            var context = new Dictionary<object, object>();
            return Clone(context);
        }

        /// <summary>
        /// Clones the instance using special context and returns cloned instance.
        /// </summary>
        /// <param name="context">Special context for providing references continuity.</param>
        /// <returns>Cloned instance.</returns>
        public LogicalModalityExpressionNode Clone(Dictionary<object, object> context)
        {
            if (context.ContainsKey(this))
            {
                return (LogicalModalityExpressionNode)context[this];
            }

            var result = new LogicalModalityExpressionNode();
            context[this] = result;

            result.Kind = Kind;
            result.KindOfOperator = KindOfOperator;
            result.Left = Left?.Clone(context);
            result.Right = Right?.Clone(context);
            result.Value = Value?.CloneValue(context);

            result.AppendAnnotations(this, context);

            return result;
        }

        /// <inheritdoc/>
        protected override ulong CalculateLongHashCode(CheckDirtyOptions options)
        {
            var result = base.CalculateLongHashCode(options);

            switch(Kind)
            {
                case KindOfLogicalModalityExpressionNode.BlankIdentifier:
                    break;

                case KindOfLogicalModalityExpressionNode.BinaryOperator:
                    Left.CheckDirty(options);
                    Right.CheckDirty(options);
                    result ^= Left.GetLongHashCode();
                    result ^= Right.GetLongHashCode();
                    break;

                case KindOfLogicalModalityExpressionNode.UnaryOperator:
                    Left.CheckDirty(options);
                    result ^= Left.GetLongHashCode();
                    break;

                case KindOfLogicalModalityExpressionNode.Value:
                    Value.CheckDirty(options);
                    result ^= Value.GetLongHashCode();
                    break;

                case KindOfLogicalModalityExpressionNode.Group:
                    Left.CheckDirty(options);
                    result ^= Left.GetLongHashCode();
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(Kind), Kind, null);
            }

            return result;
        }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}{nameof(Kind)} = {Kind}");
            sb.AppendLine($"{spaces}{nameof(KindOfOperator)} = {KindOfOperator}");

            sb.PrintObjProp(n, nameof(Left), Left);
            sb.PrintObjProp(n, nameof(Right), Right);

            sb.PrintObjProp(n, nameof(Value), Value);

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

            sb.PrintShortObjProp(n, nameof(Left), Left);
            sb.PrintShortObjProp(n, nameof(Right), Right);

            sb.PrintShortObjProp(n, nameof(Value), Value);

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

            sb.PrintExisting(n, nameof(Left), Left);
            sb.PrintExisting(n, nameof(Right), Right);

            sb.PrintBriefObjProp(n, nameof(Value), Value);

            sb.Append(base.PropertiesToBriefString(n));
            return sb.ToString();
        }

        public string ToHumanizedString(HumanizedOptions options = HumanizedOptions.ShowAll)
        {
            return DebugHelperForLogicalModalityExpression.ToString(this, options);
        }
    }
}