/*MIT License

Copyright (c) 2020 - 2024 Sergiy Tolkachov

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
using SymOntoClay.Monitor.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Storage.InheritanceStoraging
{
    public class ConsolidatedPublicFactsInheritanceStorage : BaseComponent, IInheritanceStorage
    {
        public ConsolidatedPublicFactsInheritanceStorage(ConsolidatedPublicFactsStorage parent, IMonitorLogger logger, KindOfStorage kind)
            : base(logger)
        {
            _kind = kind;
            _parent = parent;
        }

        private readonly object _lockObj = new object();
        private readonly ConsolidatedPublicFactsStorage _parent;
        private readonly List<IInheritanceStorage> _inheritanceStorages = new List<IInheritanceStorage>();
        private readonly KindOfStorage _kind;

        /// <inheritdoc/>
        public KindOfStorage Kind => _kind;

        /// <inheritdoc/>
        public IStorage Storage => _parent;

        public void AddConsolidatedStorage(IMonitorLogger logger, IInheritanceStorage storage)
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

        public void RemoveConsolidatedStorage(IMonitorLogger logger, IInheritanceStorage storage)
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
        public IList<WeightedInheritanceResultItem<InheritanceItem>> GetItemsDirectly(IMonitorLogger logger, TypeInfo subType)
        {
            lock(_lockObj)
            {
                var result = new List<WeightedInheritanceResultItem<InheritanceItem>>();

                foreach(var storage in _inheritanceStorages)
                {
                    var targetList = storage.GetItemsDirectly(logger, subType);

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
        public void RemoveInheritance(IMonitorLogger logger, InheritanceItem inheritanceItem)
        {
            throw new NotImplementedException("E72700C3-322E-438C-9C25-1093A551657B");
        }

        /// <inheritdoc/>
        public void SetInheritance(IMonitorLogger logger, InheritanceItem inheritanceItem)
        {
            throw new NotImplementedException("1C4EC832-917D-4318-9E8B-F27C2C820BE3");
        }

        /// <inheritdoc/>
        public void SetInheritance(IMonitorLogger logger, InheritanceItem inheritanceItem, bool isPrimary)
        {
            throw new NotImplementedException("6B2C32A3-0007-44F6-9740-CA24DF45DFEE");
        }

#if DEBUG
        public void DbgPrintInheritances(IMonitorLogger logger)
        {
            throw new NotImplementedException("B3A97848-26FF-4051-960C-BAB291EE79E8");
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
