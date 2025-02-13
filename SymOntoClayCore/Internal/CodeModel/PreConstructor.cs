using SymOntoClay.Core.Internal.CodeModel.Helpers;
using System.Collections.Generic;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class PreConstructor : Function
    {
        public PreConstructor()
        {
            TypeOfAccess = TypeOfAccess.Public;
            IsAnonymous = true;
            Name = NameHelper.CreateEntityName();
        }

        /// <inheritdoc/>
        public override KindOfCodeEntity Kind => KindOfCodeEntity.PreConstructor;

        /// <inheritdoc/>
        public override bool IsPreConstructor => true;

        /// <inheritdoc/>
        public override PreConstructor AsPreConstructor => this;

        /// <inheritdoc/>
        public override CodeItem CloneCodeItem(Dictionary<object, object> context)
        {
            return Clone(context);
        }

        /// <include file = "..\CommonDoc.xml" path='extradoc/method[@name="Clone"]/*' />
        public PreConstructor Clone()
        {
            var context = new Dictionary<object, object>();
            return Clone(context);
        }

        /// <include file = "..\CommonDoc.xml" path='extradoc/method[@name="CloneWithContext"]/*' />
        public PreConstructor Clone(Dictionary<object, object> context)
        {
            if (context.ContainsKey(this))
            {
                return (PreConstructor)context[this];
            }

            var result = new PreConstructor();
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
