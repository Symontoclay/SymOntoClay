using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public abstract class CodeItemDirective: AnnotatedItem
    {        
        public abstract KindOfCodeItemDirective KindOfCodeItemDirective { get; }

        public virtual bool IsSetDefaultStateDirective => false;
        public virtual SetDefaultStateDirective AsSetDefaultStateDirective => null;

        public virtual bool IsSetStateDirective => false;
        public virtual SetStateDirective AsSetStateDirective => null;

        /// <inheritdoc/>
        public override AnnotatedItem CloneAnnotatedItem(Dictionary<object, object> context)
        {
            return CloneCodeItemDirective(context);
        }

        /// <summary>
        /// Clones the instance and returns cloned instance.
        /// </summary>
        /// <returns>Cloned instance.</returns>
        public CodeItemDirective CloneCodeItemDirective()
        {
            var context = new Dictionary<object, object>();
            return CloneCodeItemDirective(context);
        }

        /// <summary>
        /// Clones the instance using special context and returns cloned instance.
        /// </summary>
        /// <param name="context">Special context for providing references continuity.</param>
        /// <returns>Cloned instance.</returns>
        public abstract CodeItemDirective CloneCodeItemDirective(Dictionary<object, object> context);

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(KindOfCodeItemDirective)} = {KindOfCodeItemDirective}");

            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(KindOfCodeItemDirective)} = {KindOfCodeItemDirective}");

            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(KindOfCodeItemDirective)} = {KindOfCodeItemDirective}");

            sb.Append(base.PropertiesToBriefString(n));
            return sb.ToString();
        }
    }
}
