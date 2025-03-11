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
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.Monitor.Common;
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

        private Dictionary<StrongIdentifierValue, List<IdleActionItem>> _itemsDict = new Dictionary<StrongIdentifierValue, List<IdleActionItem>>();

        /// <inheritdoc/>
        public void Append(IMonitorLogger logger, IdleActionItem item)
        {
            lock(_lockObj)
            {
                AnnotatedItemHelper.CheckAndFillUpHolder(logger, item, _realStorageContext.MainStorageContext.CommonNamesStorage);

                item.CheckDirty();

                var holder = item.Holder;

                if (_itemsDict.ContainsKey(holder))
                {
                    var targetList = _itemsDict[holder];


                    targetList.Add(item);
                }
                else
                {
                    _itemsDict[holder] = new List<IdleActionItem>() { item };
                }
            }
        }

        private static List<WeightedInheritanceResultItem<IdleActionItem>> _emptyIdleActionsList = new List<WeightedInheritanceResultItem<IdleActionItem>>();

        /// <inheritdoc/>
        public IList<WeightedInheritanceResultItem<IdleActionItem>> GetIdleActionsDirectly(IMonitorLogger logger, IList<WeightedInheritanceItem> weightedInheritanceItems)
        {
            lock(_lockObj)
            {
                if(_realStorageContext.Disabled)
                {
                    return _emptyIdleActionsList;
                }

                var result = new List<WeightedInheritanceResultItem<IdleActionItem>>();

                foreach (var weightedInheritanceItem in weightedInheritanceItems)
                {
                    var targetHolder = weightedInheritanceItem.SuperType.Name;

                    if(_itemsDict.ContainsKey(targetHolder))
                    {
                        var targetList = _itemsDict[targetHolder];

                        foreach(var targetVal in targetList)
                        {
                            result.Add(new WeightedInheritanceResultItem<IdleActionItem>(targetVal, weightedInheritanceItem));
                        }
                    }
                }

                return result;
            }
        }
    }
}
