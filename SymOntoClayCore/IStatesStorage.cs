using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core
{
    public interface IStatesStorage : ISpecificStorage
    {
        void Append(StateDef state);
        void SetDefaultStateName(StrongIdentifierValue name);
    }
}
