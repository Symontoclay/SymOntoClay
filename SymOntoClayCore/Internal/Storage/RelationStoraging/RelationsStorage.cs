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
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.Storage.RelationStoraging
{
    public class RelationsStorage : BaseSpecificStorage, IRelationsStorage
    {
        public RelationsStorage(KindOfStorage kind, RealStorageContext realStorageContext)
            : base(kind, realStorageContext)
        {
        }

        private readonly object _lockObj = new object();
        private readonly Dictionary<StrongIdentifierValue, Dictionary<StrongIdentifierValue, Dictionary<int, List<RelationDescription>>>> _itemsDict = new Dictionary<StrongIdentifierValue, Dictionary<StrongIdentifierValue, Dictionary<int, List<RelationDescription>>>>();

        /// <inheritdoc/>
        public void Append(IMonitorLogger logger, RelationDescription relation)
        {
            lock (_lockObj)
            {
                if (relation.TypeOfAccess != TypeOfAccess.Local)
                {
                    AnnotatedItemHelper.CheckAndFillUpHolder(logger, relation, _realStorageContext.MainStorageContext.CommonNamesStorage);
                }

                relation.CheckDirty();

                var name = relation.Name;

                var paramsCount = relation.Arguments.Count;

                var holder = relation.Holder;

                if (_itemsDict.ContainsKey(holder))
                {
                    var dict = _itemsDict[holder];

                    if (dict.ContainsKey(name))
                    {
                        var targetDict = dict[name];

                        if(targetDict.ContainsKey(paramsCount))
                        {
                            var targetList = targetDict[paramsCount];

                            StorageHelper.RemoveSameItems(logger, targetList, relation);

                            targetList.Add(relation);
                        }
                        else
                        {
                            targetDict[paramsCount] = new List<RelationDescription>() { relation };
                        }
                    }
                    else
                    {
                        var targetDict = new Dictionary<int, List<RelationDescription>>();

                        dict[name] = targetDict;

                        targetDict[paramsCount] = new List<RelationDescription>() { relation };
                    }                    
                }
                else
                {
                    var dict = new Dictionary<StrongIdentifierValue, Dictionary<int, List<RelationDescription>>>();
                    _itemsDict[holder] = dict;

                    var targetDict = new Dictionary<int, List<RelationDescription>>();

                    dict[name] = targetDict;

                    targetDict[paramsCount] = new List<RelationDescription>() { relation };
                }
            }                
        }

        private static List<WeightedInheritanceResultItem<RelationDescription>> _emptyRelationsList = new List<WeightedInheritanceResultItem<RelationDescription>>();

        /// <inheritdoc/>
        public IList<WeightedInheritanceResultItem<RelationDescription>> GetRelationsDirectly(IMonitorLogger logger, StrongIdentifierValue name, int paramsCount, IList<WeightedInheritanceItem> weightedInheritanceItems)
        {
            lock (_lockObj)
            {
                if (_realStorageContext.Disabled)
                {
                    return _emptyRelationsList;
                }

                var result = new List<WeightedInheritanceResultItem<RelationDescription>>();

                foreach (var weightedInheritanceItem in weightedInheritanceItems)
                {
                    var targetHolder = weightedInheritanceItem.SuperName;

                    if (_itemsDict.ContainsKey(targetHolder))
                    {
                        var dict = _itemsDict[targetHolder];

                        if (dict.ContainsKey(name))
                        {
                            var targetDict = dict[name];

                            if (targetDict.ContainsKey(paramsCount))
                            {
                                var targetList = targetDict[paramsCount];

                                foreach (var targetVal in targetList)
                                {
                                    result.Add(new WeightedInheritanceResultItem<RelationDescription>(targetVal, weightedInheritanceItem));
                                }
                            }
                        }
                    }
                }

                return result;
            }
        }

        /// <inheritdoc/>
        protected override void OnDisposed()
        {
            _itemsDict.Clear();

            base.OnDisposed();
        }
    }
}
