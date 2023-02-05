using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace SymOntoClay.Core.Internal.Storage.ConstructorsStoraging
{
    public class ConstructorsStorage : BaseSpecificStorage, IConstructorsStorage
    {
        public ConstructorsStorage(KindOfStorage kind, RealStorageContext realStorageContext)
            : base(kind, realStorageContext)
        { 
        }

        private readonly object _constructorsLockObj = new object();
        private readonly Dictionary<StrongIdentifierValue, Dictionary<int, List<Constructor>>> _constructorsDict = new Dictionary<StrongIdentifierValue, Dictionary<int, List<Constructor>>>();

        private readonly object _preConstructorsLockObj = new object();
        private readonly Dictionary<StrongIdentifierValue, Constructor> _preConstructorsDict = new Dictionary<StrongIdentifierValue, Constructor>();

        /// <inheritdoc/>
        public void Append(Constructor constructor)
        {
            lock(_constructorsLockObj)
            {
#if DEBUG
                //Log($"constructor = {constructor}");
#endif

                constructor.CheckDirty();

                var paramsCountList = StorageHelper.GetParamsCountList(constructor);

#if DEBUG
                //Log($"paramsCountList = {paramsCountList.WritePODListToString()}");
#endif

                var targetDict = GetDictByHolders(constructor);

                foreach (var count in paramsCountList)
                {
                    if (targetDict.ContainsKey(count))
                    {
                        var targetList = targetDict[count];

#if DEBUG
                        //Log($"targetList.Count = {targetList.Count}");
                        //Log($"constructor.GetLongHashCode() = {constructor.GetLongHashCode()}");
                        //Log($"targetList.FirstOrDefault()?.GetLongHashCode() = {targetList.FirstOrDefault()?.GetLongHashCode()}");
#endif

                        StorageHelper.RemoveSameItems(targetList, constructor);

#if DEBUG
                        //Log($"targetList.Count (after) = {targetList.Count}");
#endif

                        targetList.Add(constructor);
                    }
                    else
                    {
                        targetDict[count] = new List<Constructor>() { constructor };
                    }
                }
            }
        }

        private Dictionary<int, List<Constructor>> GetDictByHolders(Constructor constructor)
        {
            var holder = constructor.Holder;

#if DEBUG
            //Log($"holder = {holder}");
#endif

            if(_constructorsDict.ContainsKey(holder))
            {
                return _constructorsDict[holder];
            }

            var dict = new Dictionary<int, List<Constructor>>();
            _constructorsDict[holder] = dict;
            return dict;
        }

        /// <inheritdoc/>
        public IList<WeightedInheritanceResultItem<Constructor>> GetConstructorsDirectly(int paramsCount, IList<WeightedInheritanceItem> weightedInheritanceItems)
        {
            lock (_constructorsLockObj)
            {
#if DEBUG
                //Log($"GetHashCode() = {GetHashCode()}");
                //Log($"paramsCount = {paramsCount}");
#endif

                var result = new List<WeightedInheritanceResultItem<Constructor>>();

                foreach (var weightedInheritanceItem in weightedInheritanceItems)
                {
                    var targetHolder = weightedInheritanceItem.SuperName;

#if DEBUG
                    //Log($"targetHolder = {targetHolder}");
#endif

                    if(_constructorsDict.ContainsKey(targetHolder))
                    {
                        var targetDict = _constructorsDict[targetHolder];

                        if (targetDict.ContainsKey(paramsCount))
                        {
                            var targetList = targetDict[paramsCount];

                            foreach (var targetVal in targetList)
                            {
                                result.Add(new WeightedInheritanceResultItem<Constructor>(targetVal, weightedInheritanceItem));
                            }
                        }
                    }
                }

                return result;
            }
        }

        /// <inheritdoc/>
        public void AppendPreConstructor(Constructor preConstructor)
        {
            lock(_preConstructorsLockObj)
            {
#if DEBUG
                //Log($"preConstructor = {preConstructor}");
#endif

                preConstructor.CheckDirty();

                _preConstructorsDict[preConstructor.Holder] = preConstructor;
            }
        }

        /// <inheritdoc/>
        public IList<WeightedInheritanceResultItem<Constructor>> GetPreConstructorsDirectly(IList<WeightedInheritanceItem> weightedInheritanceItems)
        {
            lock (_preConstructorsLockObj)
            {
                var result = new List<WeightedInheritanceResultItem<Constructor>>();

                foreach (var weightedInheritanceItem in weightedInheritanceItems)
                {
                    var targetHolder = weightedInheritanceItem.SuperName;

#if DEBUG
                    //Log($"targetHolder = {targetHolder}");
#endif

                    if(_preConstructorsDict.ContainsKey(targetHolder))
                    {
                        var targetVal = _preConstructorsDict[targetHolder];

                        result.Add(new WeightedInheritanceResultItem<Constructor>(targetVal, weightedInheritanceItem));
                    }
                }

                return result;
            }
        }

        /// <inheritdoc/>
        protected override void OnDisposed()
        {
            _constructorsDict.Clear();
            _preConstructorsDict.Clear();

            base.OnDisposed();
        }
    }
}
