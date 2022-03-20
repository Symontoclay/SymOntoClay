using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class SetDefaultStateDirective: CodeItemDirective
    {
        /// <inheritdoc/>
        public override KindOfCodeItemDirective KindOfCodeItemDirective => KindOfCodeItemDirective.SetDefaultState;

        /// <inheritdoc/>
        public override bool IsSetDefaultStateDirective => true;

        /// <inheritdoc/>
        public override SetDefaultStateDirective AsSetDefaultStateDirective => this;

        public StrongIdentifierValue StateName { get; set; }

        /// <inheritdoc/>
        public override CodeItemDirective CloneCodeItemDirective(Dictionary<object, object> context)
        {
            if (context.ContainsKey(this))
            {
                return (SetDefaultStateDirective)context[this];
            }

            var result = new SetDefaultStateDirective();
            context[this] = result;

            result.StateName = StateName.Clone(context);

            result.AppendAnnotations(this);

            return result;
        }
    }
}
