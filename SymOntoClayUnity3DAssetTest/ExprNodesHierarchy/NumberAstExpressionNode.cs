using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Unity3DAsset.Test.ExprNodesHierarchy
{
    public class NumberAstExpressionNode : BaseAstExpression
    {
        /// <inheritdoc/>
        public override KindOfNode Kind => KindOfNode.NumberNode;

        public int Value { get; set; }

        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(Value)} = {Value}");

            sb.Append(base.PropertiesToString(n));

            return sb.ToString();
        }

        public override string GetDbgString()
        {
            return Value.ToString();
        }

        public override object Calc()
        {
            return Value;
        }
    }
}
