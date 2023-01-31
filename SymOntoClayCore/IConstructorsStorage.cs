using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core
{
    public interface IConstructorsStorage : ISpecificStorage
    {
        void Append(Constructor constructor);
    }
}
