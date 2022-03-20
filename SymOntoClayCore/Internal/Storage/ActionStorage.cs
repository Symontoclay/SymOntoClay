using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Storage
{
    public class ActionStorage : RealStorage
    {
        public ActionStorage(RealStorageSettings settings)
            : base(KindOfStorage.Object, settings)
        {
        }
    }
}
