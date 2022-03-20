using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Storage
{
    public class ObjectStorage : RealStorage
    {
        public ObjectStorage(RealStorageSettings settings)
            : base(KindOfStorage.Object, settings)
        {
        }
    }
}
