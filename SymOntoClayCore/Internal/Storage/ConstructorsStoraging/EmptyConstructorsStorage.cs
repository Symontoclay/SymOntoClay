using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Storage.ConstructorsStoraging
{
    public class EmptyConstructorsStorage : BaseEmptySpecificStorage, IConstructorsStorage
    {
        public EmptyConstructorsStorage(IStorage storage, IEntityLogger logger)
            : base(storage, logger)
        {
        }

        /// <inheritdoc/>
        public void Append(Constructor constructor)
        {
        }
    }
}
