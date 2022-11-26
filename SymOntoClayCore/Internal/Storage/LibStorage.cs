using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Storage
{
    public class LibStorage : RealStorage
    {
        public LibStorage(RealStorageSettings settings)
            : base(KindOfStorage.World, settings)
        {
        }
    }
}
