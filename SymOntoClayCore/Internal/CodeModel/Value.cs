using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public abstract class Value: AnnotatedItem
    {
        public abstract KindOfValue Kind { get; }

        public virtual bool IsLogicalValue => false;
        public virtual LogicalValue AsLogicalValue => null;

        public virtual bool IsNumberValue => false;
        public virtual NumberValue AsNumberValue => null;

        public abstract Value CloneValue();

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}{nameof(Kind)} = {Kind}");
            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}{nameof(Kind)} = {Kind}");
            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}{nameof(Kind)} = {Kind}");
            sb.Append(base.PropertiesToBriefString(n));
            return sb.ToString();
        }
    }
}
