using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Storage
{
    public class HostStorage: RealStorage
    {
        public HostStorage(IEntityLogger logger, IEntityDictionary entityDictionary)
            : base(KindOfStorage.Host, logger, entityDictionary)
        {
        }
    }
}
