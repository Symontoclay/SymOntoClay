using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel.Ast.Statements
{
    public class AstRejectStatement : AstStatement
    {
        /// <inheritdoc/>
        public override KindOfAstStatement Kind => KindOfAstStatement.RejectStatement;

        /// <inheritdoc/>
        public override AnnotatedItem CloneAnnotatedItem(Dictionary<object, object> context)
        {
            return Clone(context);
        }

        /// <inheritdoc/>
        public override AstStatement CloneAstStatement(Dictionary<object, object> context)
        {
            return Clone(context);
        }

        /// <summary>
        /// Clones the instance and returns cloned instance.
        /// </summary>
        /// <returns>Cloned instance.</returns>
        public AstRejectStatement Clone()
        {
            var context = new Dictionary<object, object>();
            return Clone(context);
        }

        /// <summary>
        /// Clones the instance using special context and returns cloned instance.
        /// </summary>
        /// <param name="context">Special context for providing references continuity.</param>
        /// <returns>Cloned instance.</returns>
        public AstRejectStatement Clone(Dictionary<object, object> context)
        {
            if (context.ContainsKey(this))
            {
                return (AstRejectStatement)context[this];
            }

            var result = new AstRejectStatement();
            context[this] = result;

            result.AppendAnnotations(this, context);

            return result;
        }
    }
}
