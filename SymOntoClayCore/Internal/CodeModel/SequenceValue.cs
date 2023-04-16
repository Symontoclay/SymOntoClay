using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class SequenceValue : Value
    {
        public SequenceValue(Value initialValue)
        {
            _values.Add(initialValue);
        }

        /// <inheritdoc/>
        public override KindOfValue KindOfValue => KindOfValue.SequenceValue;

        /// <inheritdoc/>
        public override bool IsSequenceValue => true;

        /// <inheritdoc/>
        public override SequenceValue AsSequenceValue => this;

        private readonly List<Value> _values = new List<Value>();
    }
}
