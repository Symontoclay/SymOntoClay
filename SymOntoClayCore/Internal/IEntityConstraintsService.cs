using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal
{
    public interface IEntityConstraintsService
    {
        void Init();
        IList<StrongIdentifierValue> GetConstraintsList();
        EntityConstraints ConvertToEntityConstraint(StrongIdentifierValue name);
    }
}
