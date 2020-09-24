using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.IndexedData
{
    public class IndexedLogicalSearchResultValue : IndexedValue
    {
        /// <inheritdoc/>
        public override KindOfValue KindOfValue => KindOfValue.LogicalSearchResultValue;

        /// <inheritdoc/>
        public override bool IsLogicalSearchResultValue => true;

        /// <inheritdoc/>
        public override IndexedLogicalSearchResultValue AsLogicalSearchResultValue => this;

        public LogicalSearchResultValue OriginalLogicalSearchResultValue { get; set; }

        /// <inheritdoc/>
        public override Value OriginalValue => OriginalLogicalSearchResultValue;

        public LogicalSearchResult LogicalSearchResult { get; set; }

        /// <inheritdoc/>
        public override object GetSystemValue()
        {
            return LogicalSearchResult;
        }

        /// <inheritdoc/>
        protected override ulong CalculateLongHashCode()
        {
            return base.CalculateLongHashCode() ^ LogicalSearchResult.GetLongHashCode();
        }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintObjProp(n, nameof(LogicalSearchResult), LogicalSearchResult);

            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintShortObjProp(n, nameof(LogicalSearchResult), LogicalSearchResult);

            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintBriefObjProp(n, nameof(LogicalSearchResult), LogicalSearchResult);

            sb.Append(base.PropertiesToBriefString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToDbgString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            return $"{spaces} < Logical Search result >";
        }
    }
}
