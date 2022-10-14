using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Storage.IdleActionItemsStoraging
{
    public class IdleActionItemsStorage : BaseSpecificStorage, IIdleActionItemsStorage
    {
        public IdleActionItemsStorage(KindOfStorage kind, RealStorageContext realStorageContext)
            : base(kind, realStorageContext)
        {
        }

        private readonly object _lockObj = new object();

        /// <inheritdoc/>
        public void Append(IdleActionItem item)
        {
            throw new NotImplementedException();
        }
    }
}
