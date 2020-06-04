using SymOntoClay.Core.Internal.CodeModel;
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
        public override KindOfValue Kind => KindOfValue.LogicalValue;

        /// <inheritdoc/>
        public override bool IsLogicalValue => true;

        /// <inheritdoc/>
        public override IndexedLogicalValue AsLogicalValue => this;
    }
}
