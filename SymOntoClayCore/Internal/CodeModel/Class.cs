using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class Class : CodeItem
    {
        public Class()
        {
            TypeOfAccess = TypeOfAccess.Public;
        }

        /// <inheritdoc/>
        public override KindOfCodeEntity Kind => KindOfCodeEntity.Class;

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
        public Class Clone()
        {
            var context = new Dictionary<object, object>();
            return Clone(context);
        }

        /// <summary>
        /// Clones the instance using special context and returns cloned instance.
        /// </summary>
        /// <param name="context">Special context for providing references continuity.</param>
        /// <returns>Cloned instance.</returns>
        public Class Clone(Dictionary<object, object> context)
        {
            if (context.ContainsKey(this))
            {
                return (Class)context[this];
            }

            var result = new Class();
            context[this] = result;

            result.AppendCodeItem(this, context);

            return result;
        }
    }
}
