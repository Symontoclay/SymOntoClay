using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class ResolveToTypeAttribute: Attribute
    {
        public ResolveToTypeAttribute(Type targetType)
        {
            TargetType = targetType;
        }

        public Type TargetType { get; private set; }
    }
}
