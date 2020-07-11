using SymOntoClay.Core.Internal.CodeModel;
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
    }
}
