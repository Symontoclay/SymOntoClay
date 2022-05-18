using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Core.Internal.IndexedData;
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
        public void Append(RelationDescription relation)
        {
            lock (_lockObj)
            {
#if DEBUG
                //Log($"relation = {relation}");
#endif

                if (relation.TypeOfAccess != TypeOfAccess.Local)
                {
                    AnnotatedItemHelper.CheckAndFillUpHolder(relation, _realStorageContext.MainStorageContext.CommonNamesStorage);
                }

                relation.CheckDirty();

                var name = relation.Name;

                var paramsCount = relation.Arguments.Count;

#if DEBUG
                //Log($"paramsCount = {paramsCount}");
#endif

                var holder = relation.Holder;

#if DEBUG
                //Log($"holder = {holder}");
#endif

                if (_itemsDict.ContainsKey(holder))
                {
                    var dict = _itemsDict[holder];

                    if (dict.ContainsKey(name))
                    {
                        var targetDict = dict[name];

                        if(targetDict.ContainsKey(paramsCount))
                        {
                            var targetList = targetDict[paramsCount];

                            StorageHelper.RemoveSameItems(targetList, relation);

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

        /// <inheritdoc/>
        public IList<WeightedInheritanceResultItem<RelationDescription>> GetRelationsDirectly(StrongIdentifierValue name, int paramsCount, IList<WeightedInheritanceItem> weightedInheritanceItems)
        {
            lock (_lockObj)
            {
#if DEBUG
                //Log($"name = {name}");
                //Log($"paramsCount = {paramsCount}");
#endif

                var result = new List<WeightedInheritanceResultItem<RelationDescription>>();

                foreach (var weightedInheritanceItem in weightedInheritanceItems)
                {
                    var targetHolder = weightedInheritanceItem.SuperName;

#if DEBUG
                    //Log($"targetHolder = {targetHolder}");
#endif

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
