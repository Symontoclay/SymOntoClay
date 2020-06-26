using SymOntoClay.Core.Internal.CodeModel.Ast.Expressions;
using SymOntoClay.Core.Internal.IndexedData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.DataResolvers
{
    public class OperatorsResolver: BaseResolver
    {
        public OperatorsResolver(IMainStorageContext context)
            : base(context)
        {
        }

        public IndexedOperator GetOperator(KindOfOperator kindOfOperator, IStorage storage)
        {
#if DEBUG
            Log($"kindOfOperator = {kindOfOperator}");
#endif

            var storagesList = GetStoragesList(storage);

#if DEBUG
            Log($"storagesList.Count = {storagesList.Count}");
            foreach(var tmpStorage in storagesList)
            {
                Log($"tmpStorage.Key = {tmpStorage.Key}; tmpStorage.Value.Kind = '{tmpStorage.Value.Kind}'");
            }
#endif

            var rawList = GetRawList(kindOfOperator, storagesList);

#if DEBUG
            Log($"rawList.Count = {rawList.Count}");
            foreach(var tmpItem in rawList)
            {
                Log($"tmpItem.Key = {tmpItem.Key}");
                Log($"tmpItem.Value = {tmpItem.Value}");
            }
#endif

            var filteredList = Filter(rawList);

#if DEBUG
            Log($"filteredList.Count = {filteredList.Count}");
            foreach (var tmpFilteredItem in filteredList)
            {
                Log($"tmpFilteredItem.Key = {tmpFilteredItem.Key}");
                Log($"tmpFilteredItem.Value = {tmpFilteredItem.Value}");
            }
#endif

            var targetOp = ChooseTargetItem(filteredList);

            return targetOp;
        }

        private List<KeyValuePair<uint, IndexedOperator>> GetRawList(KindOfOperator kindOfOperator, List<KeyValuePair<uint, IStorage>> storagesList)
        {
#if DEBUG
            Log($"kindOfOperator = {kindOfOperator}");
#endif

            if(!storagesList.Any())
            {
                return new List<KeyValuePair<uint, IndexedOperator>>();
            }

            var result = new List<KeyValuePair<uint, IndexedOperator>>();

            foreach(var storageItem in storagesList)
            {
                var operatorsList = storageItem.Value.OperatorsStorage.GetOperatorsDirectly(kindOfOperator);

                if(!operatorsList.Any())
                {
                    continue;
                }

                var distance = storageItem.Key;

                foreach(var op in operatorsList)
                {
                    result.Add(new KeyValuePair<uint, IndexedOperator>(distance, op));
                }
            }

            return result;
        }
    }
}
