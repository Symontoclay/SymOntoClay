using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Storage
{
    public class WorldStorage: RealStorage
    {
        public WorldStorage(IEntityLogger logger, IEntityDictionary entityDictionary)
            : base(KindOfStorage.World, logger, entityDictionary)
        {
        }
    }
}
