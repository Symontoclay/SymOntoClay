using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core
{
    public interface IStandaloneStorage
    {
        IStorage Storage { get; }
    }
}
