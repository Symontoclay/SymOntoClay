using SymOntoClay.Core.Internal.CodeModel;
using System.Collections.Generic;

namespace SymOntoClay.Core.Internal.CodeExecution
{
    public interface IReturnable
    {
        IList<StrongIdentifierValue> TypesList { get; }
    }
}
