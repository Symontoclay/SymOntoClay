using SymOntoClay.Core.Internal.Parsing.Internal.ExprLinking;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Unity3DAsset.Test.ExprNodesHierarchy
{
    public class TstBinaryAstExpression : TstBaseAstExpression
    {
        /// <inheritdoc/>
        public override TstKindOfNode Kind => TstKindOfNode.BinaryOperator;

        public TstKindOfOperator KindOfOperator { get; set; } = TstKindOfOperator.Unknown;

        public TstBaseAstExpression Left { get; set; }
        public TstBaseAstExpression Right { get; set; }

        protected override IAstNode NLeft { get => Left; set => Left = (TstBaseAstExpression)value; }
        protected override IAstNode NRight { get => Right; set => Right = (TstBaseAstExpression)value; }

        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(KindOfOperator)} = {KindOfOperator}");

            sb.PrintObjProp(n, nameof(Left), Left);
            sb.PrintObjProp(n, nameof(Right), Right);

            sb.Append(base.PropertiesToString(n));

            return sb.ToString();
        }

        public override string GetDbgString()
        {
            var mark = string.Empty;

            switch (KindOfOperator)
            {
                case TstKindOfOperator.Plus:
                    mark = "+";
                    break;

                case TstKindOfOperator.Minus:
                    mark = "-";
                    break;

                case TstKindOfOperator.Mul:
                    mark = "*";
                    break;

                case TstKindOfOperator.Div:
                    mark = "/";
                    break;

                case TstKindOfOperator.Assign:
                    mark = "=";
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(KindOfOperator), KindOfOperator, null);
            }

            return $"{Left?.GetDbgString()} {mark} {Right?.GetDbgString()}";
        }

        public override object Calc()
        {
            switch (KindOfOperator)
            {
                case TstKindOfOperator.Plus:
                    return (int)Left.Calc() + (int)Right.Calc();

                case TstKindOfOperator.Minus:
                    return (int)Left.Calc() - (int)Right.Calc();

                case TstKindOfOperator.Mul:
                    return (int)Left.Calc() * (int)Right.Calc();

                case TstKindOfOperator.Div:
                    return (int)Left.Calc() / (int)Right.Calc();

                case TstKindOfOperator.Assign:
                    {
                        var val = (int)Right.Calc();

                        if (Left is TstVarAstExpression)
                        {
                            ((TstVarAstExpression)Left).Value = val;
                        }

                        return val;
                    }

                default:
                    throw new ArgumentOutOfRangeException(nameof(KindOfOperator), KindOfOperator, null);
            }
        }
    }
}
