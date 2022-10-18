using SymOntoClay.Core.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel.Ast.Statements
{
    public class AstWaitStatement : AstStatement
    {
        /// <inheritdoc/>
        public override KindOfAstStatement Kind => KindOfAstStatement.WaitStatement;

        /// <inheritdoc/>
        public override AnnotatedItem CloneAnnotatedItem(Dictionary<object, object> context)
        {
            return CloneAstStatement(context);
        }

        /// <inheritdoc/>
        public override AstStatement CloneAstStatement(Dictionary<object, object> context)
        {
            if (context.ContainsKey(this))
            {
                return (AstStatement)context[this];
            }

            var result = new AstWaitStatement();
            context[this] = result;

            result.AppendAnnotations(this, context);

            return result;
        }

        /// <inheritdoc/>
        public override string ToHumanizedString(DebugHelperOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
