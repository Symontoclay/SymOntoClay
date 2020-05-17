using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Storage
{
    public class LocalStorage: RealStorage
    {
        public LocalStorage(RealStorageSettings settings)
            : base(KindOfStorage.Local, settings)
        {
        }
    }
}
