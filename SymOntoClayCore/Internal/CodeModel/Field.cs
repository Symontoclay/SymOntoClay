using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class Field: Var
    {
        public Field()
        {
            TypeOfAccess = TypeOfAccess.Public;
        }

        /// <inheritdoc/>
        public override KindOfCodeEntity Kind => KindOfCodeEntity.Field;

        /// <inheritdoc/>
        public override bool IsField => true;

        /// <inheritdoc/>
        public override Field AsField => this;

        /// <inheritdoc/>
        public override CodeItem CloneCodeItem()
        {
            return Clone();
        }

        /// <inheritdoc/>
        public override CodeItem CloneCodeItem(Dictionary<object, object> cloneContext)
        {
            return Clone(cloneContext);
        }

        /// <inheritdoc/>
        public override AnnotatedItem CloneAnnotatedItem(Dictionary<object, object> context)
        {
            return Clone(context);
        }

        /// <summary>
        /// Clones the instance and returns cloned instance.
        /// </summary>
        /// <returns>Cloned instance.</returns>
        public Field Clone()
        {
            var context = new Dictionary<object, object>();
            return Clone(context);
        }

        /// <summary>
        /// Clones the instance using special context and returns cloned instance.
        /// </summary>
        /// <param name="context">Special context for providing references continuity.</param>
        /// <returns>Cloned instance.</returns>
        public Field Clone(Dictionary<object, object> context)
        {
            if (context.ContainsKey(this))
            {
                return (Field)context[this];
            }

            var result = new Field();
            context[this] = result;

            result.AppendVar(this, context);

            return result;
        }
    }
}
