using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Storage
{
    public interface IStorageComponent
    {
        IStorage GlobalStorage { get; }
    }
}
