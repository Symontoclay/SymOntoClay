using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Storage.SynonymsStorage
{
    public class EmptySynonymsStorage : BaseEmptySpecificStorage, ISynonymsStorage
    {
        public EmptySynonymsStorage(IStorage storage, IEntityLogger logger)
            : base(storage, logger)
        {
        }
    }
}
