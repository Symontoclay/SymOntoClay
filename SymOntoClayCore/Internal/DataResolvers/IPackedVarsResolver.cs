using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.DataResolvers
{
    public interface IPackedVarsResolver
    {
        Value GetVarValue(StrongIdentifierValue varName);
    }
}
