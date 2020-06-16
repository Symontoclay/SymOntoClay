using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Unity3DAsset.Test.ExprNodesHierarchy
{
    public class TstNumberAstExpressionNode : TstBaseAstExpression
    {
        /// <inheritdoc/>
        public override TstKindOfNode Kind => TstKindOfNode.NumberNode;

        public int Value { get; set; }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(Value)} = {Value}");

            sb.Append(base.PropertiesToString(n));

            return sb.ToString();
        }

        /// <inheritdoc/>
        public override string GetDbgString()
        {
            return Value.ToString();
        }

        /// <inheritdoc/>
        public override object Calc()
        {
            return Value;
        }
    }
}
