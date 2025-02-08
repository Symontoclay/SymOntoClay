using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Storage
{
    public class CompoundTaskInstanceStorage : RealStorage
    {
        public CompoundTaskInstanceStorage(RealStorageSettings settings)
            : base(KindOfStorage.CompoundTaskInstance, settings)
        {
        }
    }
}
