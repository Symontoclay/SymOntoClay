﻿using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Storage.FuzzyLogic
{
    public class FuzzyLogicStorage : BaseLoggedComponent, IFuzzyLogicStorage
    {
        public FuzzyLogicStorage(KindOfStorage kind, RealStorageContext realStorageContext)
            : base(realStorageContext.MainStorageContext.Logger)
        {
            _kind = kind;
            _realStorageContext = realStorageContext;
        }

        private readonly object _lockObj = new object();

        private readonly KindOfStorage _kind;

        /// <inheritdoc/>
        public KindOfStorage Kind => _kind;

        private readonly RealStorageContext _realStorageContext;

        /// <inheritdoc/>
        public IStorage Storage => _realStorageContext.Storage;
    }
}
