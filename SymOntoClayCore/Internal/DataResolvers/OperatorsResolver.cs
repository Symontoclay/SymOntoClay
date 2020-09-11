using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel.Ast.Expressions;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.CoreHelper.DebugHelpers;
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

        public IndexedOperator GetOperator(KindOfOperator kindOfOperator, LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
#if DEBUG
            //Log($"kindOfOperator = {kindOfOperator}");
#endif

            var storage = localCodeExecutionContext.Storage;

            var storagesList = GetStoragesList(storage);

#if DEBUG
            //Log($"storagesList.Count = {storagesList.Count}");
            //foreach(var tmpStorage in storagesList)
            //{
            //    Log($"tmpStorage.Key = {tmpStorage.Key}; tmpStorage.Value.Kind = '{tmpStorage.Value.Kind}'");
            //}
#endif

            var inheritanceResolver = _context.DataResolversFactory.GetInheritanceResolver();

            var optionsForInheritanceResolver = options.Clone();
            optionsForInheritanceResolver.AddSelf = true;

            var weightedInheritanceItems = inheritanceResolver.GetWeightedInheritanceItems(localCodeExecutionContext, optionsForInheritanceResolver);

#if DEBUG
            //Log($"weightedInheritanceItems = {weightedInheritanceItems.WriteListToString()}");
#endif

            var rawList = GetRawList(kindOfOperator, storagesList, weightedInheritanceItems);

#if DEBUG
            //Log($"rawList = {rawList.WriteListToString()}");
#endif

            var filteredList = Filter(rawList);

#if DEBUG
            //Log($"filteredList = {filteredList.WriteListToString()}");
#endif

            var targetOp = ChooseTargetItem(filteredList);

            return targetOp;
        }

        private List<WeightedInheritanceResultItemWithStorageInfo<IndexedOperator>> GetRawList(KindOfOperator kindOfOperator, List<StorageUsingOptions> storagesList, IList<WeightedInheritanceItem> weightedInheritanceItems)
        {
#if DEBUG
            //Log($"kindOfOperator = {kindOfOperator}");
#endif

            if(!storagesList.Any())
            {
                return new List<WeightedInheritanceResultItemWithStorageInfo<IndexedOperator>>();
            }

            var result = new List<WeightedInheritanceResultItemWithStorageInfo<IndexedOperator>>();

            foreach(var storageItem in storagesList)
            {
                var operatorsList = storageItem.Storage.OperatorsStorage.GetOperatorsDirectly(kindOfOperator, weightedInheritanceItems);

                if(!operatorsList.Any())
                {
                    continue;
                }

                var distance = storageItem.Priority;
                var storage = storageItem.Storage;

                foreach(var op in operatorsList)
                {
                    result.Add(new WeightedInheritanceResultItemWithStorageInfo<IndexedOperator>(op, distance, storage));
                }
            }

            return result;
        }
    }
}
