using SymOntoClay.Core.Internal.CodeModel.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class Constructor : Function
    {
        public Constructor()
        {
            TypeOfAccess = DefaultTypeOfAccess;
            IsAnonymous = true;
            Name = NameHelper.CreateEntityName();
        }

        /// <inheritdoc/>
        public override KindOfCodeEntity Kind => KindOfCodeEntity.Constructor;

        /// <inheritdoc/>
        public override bool IsConstructor => true;

        /// <inheritdoc/>
        public override Constructor AsConstructor => this;

        /// <inheritdoc/>
        public override CodeItem CloneCodeItem(Dictionary<object, object> cloneContext)
        {
            return Clone(cloneContext);
        }

        /// <summary>
        /// Clones the instance and returns cloned instance.
        /// </summary>
        /// <returns>Cloned instance.</returns>
        public Constructor Clone()
        {
            var context = new Dictionary<object, object>();
            return Clone(context);
        }

        /// <summary>
        /// Clones the instance using special context and returns cloned instance.
        /// </summary>
        /// <param name="context">Special context for providing references continuity.</param>
        /// <returns>Cloned instance.</returns>
        public Constructor Clone(Dictionary<object, object> context)
        {
            if (context.ContainsKey(this))
            {
                return (Constructor)context[this];
            }

            var result = new Constructor();
            context[this] = result;

            result.AppendFuction(this, context);

            return result;
        }

        /// <inheritdoc/>
        public override Function CloneFunction(Dictionary<object, object> context)
        {
            return Clone(context);
        }
    }
}
