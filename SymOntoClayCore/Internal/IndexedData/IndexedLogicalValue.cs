using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace SymOntoClay.Core.Internal.IndexedData
{
    public class IndexedLogicalValue: IndexedValue
    {
        public LogicalValue OriginalLogicalValue { get; set; }

        /// <inheritdoc/>
        public override Value OriginalValue => OriginalLogicalValue;

        /// <inheritdoc/>
        public override KindOfValue KindOfValue => KindOfValue.LogicalValue;

        /// <inheritdoc/>
        public override bool IsLogicalValue => true;

        /// <inheritdoc/>
        public override IndexedLogicalValue AsLogicalValue => this;

        public float? SystemValue { get; set; }

        /// <inheritdoc/>
        public override object GetSystemValue()
        {
            return SystemValue;
        }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}{nameof(SystemValue)} = {SystemValue}");

            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}{nameof(SystemValue)} = {SystemValue}");

            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}{nameof(SystemValue)} = {SystemValue}");

            sb.Append(base.PropertiesToBriefString(n));
            return sb.ToString();
        }
    }
}
