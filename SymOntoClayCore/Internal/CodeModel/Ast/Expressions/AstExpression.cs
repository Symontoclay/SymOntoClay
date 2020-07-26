using SymOntoClay.Core.Internal.Parsing.Internal.ExprLinking;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel.Ast.Expressions
{
    public abstract class AstExpression : AnnotatedItem, IAstNode
    {
        public abstract KindOfAstExpression Kind { get; }

        protected virtual IAstNode NLeft { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        protected virtual IAstNode NRight { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        IAstNode IAstNode.Left { get => NLeft; set => NLeft = value; }
        IAstNode IAstNode.Right { get => NRight; set => NRight = value; }

        /// <summary>
        /// Clones the instance and returns cloned instance.
        /// </summary>
        /// <returns>Cloned instance.</returns>
        public AstExpression CloneAstExpression()
        {
            var context = new Dictionary<object, object>();
            return CloneAstExpression(context);
        }

        /// <summary>
        /// Clones the instance using special context and returns cloned instance.
        /// </summary>
        /// <param name="context">Special context for providing references continuity.</param>
        /// <returns>Cloned instance.</returns>
        public abstract AstExpression CloneAstExpression(Dictionary<object, object> context);

        /// <inheritdoc/>
        public override string ToString()
        {
            return ToString(0u);
        }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(Kind)} = {Kind}");

            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(Kind)} = {Kind}");

            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(Kind)} = {Kind}");

            sb.Append(base.PropertiesToBriefString(n));
            return sb.ToString();
        }
    }
}
