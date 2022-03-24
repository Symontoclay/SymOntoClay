using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Storage
{
    public class StateStorage : RealStorage
    {
        public StateStorage(RealStorageSettings settings)
            : base(KindOfStorage.State, settings)
        {
        }
    }
}
