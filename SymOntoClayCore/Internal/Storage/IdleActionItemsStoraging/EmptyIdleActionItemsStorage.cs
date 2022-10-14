using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Storage.IdleActionItemsStoraging
{
    public class EmptyIdleActionItemsStorage : BaseEmptySpecificStorage, IIdleActionItemsStorage
    {
        public EmptyIdleActionItemsStorage(IStorage storage, IEntityLogger logger)
            : base(storage, logger)
        {
        }

        /// <inheritdoc/>
        public void Append(IdleActionItem item)
        {
        }
    }
}
