using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Storage
{
    public abstract class BaseSpecificStorage : BaseComponent, ISpecificStorage
    {
        protected BaseSpecificStorage(KindOfStorage kind, RealStorageContext realStorageContext)
            : base(realStorageContext.MainStorageContext.Logger)
        {
            _kind = kind;
            _realStorageContext = realStorageContext;
            _mainStorageContext = realStorageContext.MainStorageContext;
        }

        protected readonly KindOfStorage _kind;

        /// <inheritdoc/>
        public KindOfStorage Kind => _kind;

        protected readonly RealStorageContext _realStorageContext;
        protected readonly IMainStorageContext _mainStorageContext;

        /// <inheritdoc/>
        public IStorage Storage => _realStorageContext.Storage;
    }
}
