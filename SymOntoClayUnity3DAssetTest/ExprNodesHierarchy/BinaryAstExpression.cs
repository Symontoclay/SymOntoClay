using SymOntoClay.Core.Internal.Parsing.Internal.ExprLinking;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Unity3DAsset.Test.ExprNodesHierarchy
{
    public class BinaryAstExpression : BaseAstExpression
    {
        /// <inheritdoc/>
        public override KindOfNode Kind => KindOfNode.BinaryOperator;

        public KindOfOperator KindOfOperator { get; set; } = KindOfOperator.Unknown;

        public BaseAstExpression Left { get; set; }
        public BaseAstExpression Right { get; set; }

        protected override IAstNode NLeft { get => Left; set => Left = (BaseAstExpression)value; }
        protected override IAstNode NRight { get => Right; set => Right = (BaseAstExpression)value; }

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
                case KindOfOperator.Plus:
                    mark = "+";
                    break;

                case KindOfOperator.Minus:
                    mark = "-";
                    break;

                case KindOfOperator.Mul:
                    mark = "*";
                    break;

                case KindOfOperator.Div:
                    mark = "/";
                    break;

                case KindOfOperator.Assign:
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
                case KindOfOperator.Plus:
                    return (int)Left.Calc() + (int)Right.Calc();

                case KindOfOperator.Minus:
                    return (int)Left.Calc() - (int)Right.Calc();

                case KindOfOperator.Mul:
                    return (int)Left.Calc() * (int)Right.Calc();

                case KindOfOperator.Div:
                    return (int)Left.Calc() / (int)Right.Calc();

                case KindOfOperator.Assign:
                    {
                        var val = (int)Right.Calc();

                        if (Left is VarAstExpression)
                        {
                            ((VarAstExpression)Left).Value = val;
                        }

                        return val;
                    }

                default:
                    throw new ArgumentOutOfRangeException(nameof(KindOfOperator), KindOfOperator, null);
            }
        }
    }
}
