using SymOntoClay.CoreHelper.DebugHelpers;
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

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintObjProp(n, nameof(StateName), StateName);

            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintShortObjProp(n, nameof(StateName), StateName);

            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintBriefObjProp(n, nameof(StateName), StateName);

            sb.Append(base.PropertiesToBriefString(n));
            return sb.ToString();
        }
    }
}
