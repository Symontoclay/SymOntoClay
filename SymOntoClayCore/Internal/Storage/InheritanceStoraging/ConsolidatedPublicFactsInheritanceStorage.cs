/*MIT License

Copyright (c) 2020 - 2022 Sergiy Tolkachov

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.*/

using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Storage.InheritanceStoraging
{
    public class ConsolidatedPublicFactsInheritanceStorage : BaseComponent, IInheritanceStorage
    {
        public ConsolidatedPublicFactsInheritanceStorage(ConsolidatedPublicFactsStorage parent, IEntityLogger logger)
            : base(logger)
        {
            _parent = parent;
        }

        private readonly object _lockObj = new object();
        private readonly ConsolidatedPublicFactsStorage _parent;
        private readonly List<IInheritanceStorage> _inheritanceStorages = new List<IInheritanceStorage>();

        /// <inheritdoc/>
        public KindOfStorage Kind => KindOfStorage.WorldPublicFacts;

        /// <inheritdoc/>
        public IStorage Storage => _parent;

        public void AddPublicFactsStorageOfOtherGameComponent(IInheritanceStorage storage)
        {
            lock (_lockObj)
            {
                if(_inheritanceStorages.Contains(storage))
                {
                    return;
                }

                _inheritanceStorages.Add(storage);
            }
        }

        public void RemovePublicFactsStorageOfOtherGameComponent(IInheritanceStorage storage)
        {
            lock (_lockObj)
            {
                if (!_inheritanceStorages.Contains(storage))
                {
                    return;
                }

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
