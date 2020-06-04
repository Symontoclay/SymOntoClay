using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.IndexedData
{
    public class IndexedNumberValue : IndexedValue
    {
        public NumberValue OriginalNumberValue { get; set; }

        /// <inheritdoc/>
        public override Value OriginalValue => OriginalNumberValue;

        /// <inheritdoc/>
        public override KindOfValue Kind => KindOfValue.NumberValue;

        /// <inheritdoc/>
        public override bool IsNumberValue => true;

        /// <inheritdoc/>
        public override IndexedNumberValue AsNumberValue => this;
    }
}
