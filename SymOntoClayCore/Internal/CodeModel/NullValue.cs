﻿using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class NullValue : Value
    {
        /// <inheritdoc/>
        public override KindOfValue Kind => KindOfValue.NullValue;

        /// <inheritdoc/>
        public override Value CloneValue()
        {
            var result = new NullValue();
            result.AppendAnnotations(this);

            return result;
        }
    }
}
