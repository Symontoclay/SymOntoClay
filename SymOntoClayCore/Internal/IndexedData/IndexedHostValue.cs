using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.IndexedData
{
    public class IndexedHostValue : IndexedValue
    {
        public HostValue OriginalHostValue { get; set; }

        /// <inheritdoc/>
        public override Value OriginalValue => OriginalHostValue;

        /// <inheritdoc/>
        public override KindOfValue KindOfValue => KindOfValue.HostValue;

        /// <inheritdoc/>
        public override bool IsHostValue => true;

        /// <inheritdoc/>
        public override IndexedHostValue AsHostValue => this;

        /// <inheritdoc/>
        public override object GetSystemValue()
        {
            return this;
        }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.Append(base.PropertiesToBriefString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToDbgString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            return $"{spaces}ref: @@Host";
        }
    }
}
