using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public interface IVarDecl: IAnnotatedItem
    {
        StrongIdentifierValue Name { get; }
        List<StrongIdentifierValue> TypesList { get; }
    }
}
