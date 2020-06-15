using SymOntoClay.Core.Internal.Parsing.Internal.ExprLinking;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Unity3DAsset.Test.ExprNodesHierarchy
{
    public class UnaryAstExpression : BaseAstExpression
    {
        /// <inheritdoc/>
        public override KindOfNode Kind => KindOfNode.UnaryOperator;

        public KindOfOperator KindOfOperator { get; set; } = KindOfOperator.Unknown;

        public BaseAstExpression Left { get; set; }

        protected override IAstNode NLeft { get => Left; set => Left = (BaseAstExpression)value; }

        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(KindOfOperator)} = {KindOfOperator}");

            sb.PrintObjProp(n, nameof(Left), Left);

            sb.Append(base.PropertiesToString(n));

            return sb.ToString();
        }

        public override string GetDbgString()
        {
            var mark = string.Empty;

            switch (KindOfOperator)
            {
                case KindOfOperator.Plus:
                    mark = "+";
                    break;

                case KindOfOperator.Minus:
                    mark = "-";
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(KindOfOperator), KindOfOperator, null);
            }

            return $"{mark} {Left?.GetDbgString()}";
        }

        public override object Calc()
        {
            switch (KindOfOperator)
            {
                case KindOfOperator.Plus:
                    return +(int)Left.Calc();

                case KindOfOperator.Minus:
                    return -(int)Left.Calc();

                default:
                    throw new ArgumentOutOfRangeException(nameof(KindOfOperator), KindOfOperator, null);
            }
        }
    }
}
