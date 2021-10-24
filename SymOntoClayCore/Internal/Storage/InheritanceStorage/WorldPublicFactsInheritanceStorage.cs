using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Storage.InheritanceStorage
{
    public class WorldPublicFactsInheritanceStorage : BaseComponent, IInheritanceStorage
    {
        public WorldPublicFactsInheritanceStorage(WorldPublicFactsStorage parent, IEntityLogger logger)
            : base(logger)
        {
            _parent = parent;
        }

        private readonly object _lockObj = new object();
        private readonly WorldPublicFactsStorage _parent;
        private readonly List<IInheritanceStorage> _inheritanceStorages = new List<IInheritanceStorage>();

        /// <inheritdoc/>
        public KindOfStorage Kind => KindOfStorage.WorldPublicFacts;

        /// <inheritdoc/>
        public IStorage Storage => _parent;

        public void AddPublicFactsStorageOfOtherGameComponent(IInheritanceStorage storage)
        {
            lock (_lockObj)
            {
                _inheritanceStorages.Add(storage);
            }
        }

        public void RemovePublicFactsStorageOfOtherGameComponent(IInheritanceStorage storage)
        {
            lock (_lockObj)
            {
                _inheritanceStorages.Remove(storage);
            }
        }

        /// <inheritdoc/>
        public IList<WeightedInheritanceResultItem<InheritanceItem>> GetItemsDirectly(StrongIdentifierValue subName)
        {
            lock(_lockObj)
            {
                var result = new List<WeightedInheritanceResultItem<InheritanceItem>>();

                foreach(var storage in _inheritanceStorages)
                {
                    var targetList = storage.GetItemsDirectly(subName);

                    if(targetList == null)
                    {
                        continue;
                    }

                    result.AddRange(targetList);
                }

                return result;
            }            
        }

        /// <inheritdoc/>
        public void RemoveInheritance(InheritanceItem inheritanceItem)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void SetInheritance(InheritanceItem inheritanceItem)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void SetInheritance(InheritanceItem inheritanceItem, bool isPrimary)
        {
            throw new NotImplementedException();
        }

#if DEBUG
        public void DbgPrintInheritances()
        {
            throw new NotImplementedException();
        }
#endif

        /// <inheritdoc/>
        protected override void OnDisposed()
        {
            lock (_lockObj)
            {
                _inheritanceStorages.Clear();
            }

            base.OnDisposed();
        }
    }
}
