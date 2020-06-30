using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class NullValue : Value
    {
        /// <inheritdoc/>
        public override KindOfValue KindOfValue => KindOfValue.NullValue;

        /// <inheritdoc/>
        public override bool IsNullValue => true;

        /// <inheritdoc/>
        public override NullValue AsNullValue => this;

        public IndexedNullValue Indexed { get; set; }

        /// <inheritdoc/>
        public override object GetSystemValue()
        {
            return null;
        }

        /// <inheritdoc/>
        public override Value CloneValue(Dictionary<object, object> cloneContext)
        {
            var result = new NullValue();
            result.AppendAnnotations(this, cloneContext);

            return result;
        }

        /// <inheritdoc/>
        protected override string PropertiesToDbgString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            return $"{spaces}NULL";
        }
    }
}
