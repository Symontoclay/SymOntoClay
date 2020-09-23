using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.IndexedData
{
    public class IndexedNullValue : IndexedValue
    {
        public NullValue OriginalNullValue { get; set; }

        /// <inheritdoc/>
        public override Value OriginalValue => OriginalNullValue;

        /// <inheritdoc/>
        public override KindOfValue KindOfValue => KindOfValue.NullValue;

        /// <inheritdoc/>
        public override bool IsNullValue => true;

        /// <inheritdoc/>
        public override IndexedNullValue AsNullValue => this;

        /// <inheritdoc/>
        public override object GetSystemValue()
        {
            return null;
        }

        /// <inheritdoc/>
        protected override ulong CalculateLongHashCode()
        {
            return LongHashCodeWeights.NullWeight ^ base.CalculateLongHashCode();
        }

        /// <inheritdoc/>
        protected override string PropertiesToDbgString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            return $"{spaces}NULL";
        }
    }
}
