using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Storage
{
    public interface IStorageFactory
    {
        IStorage CreateStorage(RealStorageSettings settings);
    }
}
