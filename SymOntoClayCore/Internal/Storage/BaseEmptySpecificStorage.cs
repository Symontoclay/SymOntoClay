using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Storage
{
    public abstract class BaseEmptySpecificStorage: BaseComponent, ISpecificStorage
    {
        protected BaseEmptySpecificStorage(IStorage storage, IEntityLogger logger)
            : base(logger)
        {
            _storage = storage;
        }

        private readonly IStorage _storage;

        /// <inheritdoc/>
        public KindOfStorage Kind => _storage.Kind;

        /// <inheritdoc/>
        public IStorage Storage => _storage;
    }
}
