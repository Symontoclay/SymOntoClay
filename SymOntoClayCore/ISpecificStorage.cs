using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core
{
    public interface ISpecificStorage
    {
        KindOfStorage Kind { get; }
        IStorage Storage { get; }
    }
}
