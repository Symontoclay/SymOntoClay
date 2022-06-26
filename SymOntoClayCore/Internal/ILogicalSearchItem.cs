using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal
{
    public interface ILogicalSearchItem: IReadOnlyMemberAccess
    {
        Value ObligationModality { get; }
        Value SelfObligationModality { get; }
    }
}
