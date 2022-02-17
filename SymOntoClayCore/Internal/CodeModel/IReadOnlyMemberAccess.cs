using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public interface IReadOnlyMemberAccess
    {
        TypeOfAccess TypeOfAccess { get; }
    }
}
