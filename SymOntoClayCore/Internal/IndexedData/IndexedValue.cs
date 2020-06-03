using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.IndexedData
{
    public abstract class IndexedValue: IndexedAnnotatedItem
    {
        public abstract Value OriginalValue { get; }

        public abstract KindOfValue Kind { get; }

        public virtual bool IsLogicalValue => false;
        public virtual IndexedLogicalValue AsLogicalValue => null;

        public virtual bool IsNumberValue => false;
        public virtual IndexedNumberValue AsNumberValue => null;

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}{nameof(Kind)} = {Kind}");

            sb.PrintBriefObjProp(n, nameof(OriginalValue), OriginalValue);

            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}{nameof(Kind)} = {Kind}");

            sb.PrintBriefObjProp(n, nameof(OriginalValue), OriginalValue);

            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}{nameof(Kind)} = {Kind}");

            sb.PrintBriefObjProp(n, nameof(OriginalValue), OriginalValue);

            sb.Append(base.PropertiesToBriefString(n));
            return sb.ToString();
        }
    }
}
