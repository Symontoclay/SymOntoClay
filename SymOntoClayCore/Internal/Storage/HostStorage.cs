using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Storage
{
    public class HostStorage: RealStorage
    {
        public HostStorage(RealStorageSettings settings)
            : base(KindOfStorage.Host, settings)
        {
        }
    }
}
