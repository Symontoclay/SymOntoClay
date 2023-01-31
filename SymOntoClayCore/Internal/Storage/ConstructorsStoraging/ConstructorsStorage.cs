using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Storage.ConstructorsStoraging
{
    public class ConstructorsStorage : BaseSpecificStorage, IConstructorsStorage
    {
        public ConstructorsStorage(KindOfStorage kind, RealStorageContext realStorageContext)
            : base(kind, realStorageContext)
        { 
        }

        private readonly object _lockObj = new object();

        private readonly Dictionary<StrongIdentifierValue, Dictionary<int, List<Constructor>>> _constructorsDict = new Dictionary<StrongIdentifierValue, Dictionary<int, List<Constructor>>>();

        /// <inheritdoc/>
        public void Append(Constructor constructor)
        {
            lock(_lockObj)
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
    }
}
