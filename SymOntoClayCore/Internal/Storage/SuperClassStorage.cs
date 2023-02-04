using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Storage
{
    public class SuperClassStorage : RealStorage
    {
        public SuperClassStorage(RealStorageSettings settings)
            : base(KindOfStorage.SuperClass, settings)
        {
        }
    }
}
