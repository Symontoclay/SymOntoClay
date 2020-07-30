using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public enum KindOfValue
    {
        Unknown,
        NullValue,
        LogicalValue,
        NumberValue,
        StringValue,
        StrongIdentifierValue,
        TaskValue,
        AnnotationValue
    }
}
