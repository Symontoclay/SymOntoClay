using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing.Internal.ExprLinking
{
    public class IntermediateAstNodePoint : IObjectToString
    {
        public IntermediateAstNode RootNode { get; set; }
        public IntermediateAstNode CurrentNode { get; set; }

        public T BuildExpr<T>()
        {
            return (T)BuildExpr();
        }

        public IAstNode BuildExpr()
        {
            return RootNode?.BuildExpr();
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return ToString(0u);
        }

        /// <inheritdoc/>
        public string ToString(uint n)
        {
            return this.GetDefaultToStringInformation(n);
        }

        /// <inheritdoc/>
        string IObjectToString.PropertiesToString(uint n)
        {
            return PropertiesToString(n);
        }

        protected virtual string PropertiesToString(uint n)
        {
            var sb = new StringBuilder();

            sb.PrintObjProp(n, nameof(RootNode), RootNode);
            sb.PrintObjProp(n, nameof(CurrentNode), CurrentNode);

            return sb.ToString();
        }
    }
}
