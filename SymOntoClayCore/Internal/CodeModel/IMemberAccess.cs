using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public interface IMemberAccess
    {
        StrongIdentifierValue Holder { get; set; }
        TypeOfAccess TypeOfAccess { get; set; }
    }
}
