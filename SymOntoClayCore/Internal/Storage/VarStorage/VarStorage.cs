using SymOntoClay.Core.Internal.IndexedData;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Storage.VarStorage
{
    public class VarStorage: BaseLoggedComponent, IVarStorage
    {
        public VarStorage(KindOfStorage kind, RealStorageContext realStorageContext)
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

        private Dictionary<IndexedStrongIdentifierValue, IndexedValue> _indexedSystemVariables = new Dictionary<IndexedStrongIdentifierValue, IndexedValue>();

        /// <inheritdoc/>
        public void SetSystemValue(IndexedStrongIdentifierValue varName, IndexedValue value)
        {
            lock(_lockObj)
            {
#if DEBUG
                //Log($"varName = {varName}");
                //Log($"value = {value}");
#endif

                _indexedSystemVariables[varName] = value;
            }
        }

        /// <inheritdoc/>
        public IndexedValue GetSystemValueDirectly(IndexedStrongIdentifierValue varName)
        {
            lock (_lockObj)
            {
#if DEBUG
                //Log($"varName = {varName}");
#endif

                if(_indexedSystemVariables.ContainsKey(varName))
                {
                    return _indexedSystemVariables[varName];
                }

                return null;
            }
        }
    }
}
