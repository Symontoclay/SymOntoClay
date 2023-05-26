using Newtonsoft.Json;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.DataResolvers
{
    public class LogicalSearchVarResultsItemInvertor : BaseResolver
    {
        public LogicalSearchVarResultsItemInvertor(IMainStorageContext context)
            : base(context)
        {
        }

        public IList<T> Invert<T>(IEnumerable<IResultOfQueryToRelation> source, List<StorageUsingOptions> storagesList) where T : IResultOfQueryToRelation
        {
#if DEBUG
            foreach (var tmpItem in source)
            {
                Log("&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&");

                foreach (var varInfo in tmpItem.ResultOfVarOfQueryToRelationList)
                {
                    Log($"varInfo = {varInfo}");
                }
            }
#endif

            var initialResultVarsDict = source.SelectMany(p => p.ResultOfVarOfQueryToRelationList).GroupBy(p => p.NameOfVar).ToDictionary(p => p.Key, p => p.Select(x => x.FoundExpression).ToList());

#if DEBUG
            var tmpVar = initialResultVarsDict.ToDictionary(p => p.Key.ToHumanizedString(), p => p.Value.Select(x => x.ToHumanizedString()));

            Log($"tmpVar = {JsonConvert.SerializeObject(tmpVar, Formatting.Indented)}");
#endif

            var replacingNotResultsStrategy = ReplacingNotResultsStrategy.AllKindOfItems;

            foreach (var initialResultVarsKvpItem in initialResultVarsDict)
            {
                var varName = initialResultVarsKvpItem.Key;
                var exceptList = initialResultVarsKvpItem.Value;

#if DEBUG
                Log($"varName = {varName}");
                Log($"exceptList = {exceptList.WriteListToString()}");
#endif

                var calculateTargetKindsOfItemsResult = CalculateTargetKindsOfItems(exceptList, replacingNotResultsStrategy);

                var newLogicalQueryNodes = GetLogicalQueryNodes(exceptList, calculateTargetKindsOfItemsResult.ReplacingNotResultsStrategy, calculateTargetKindsOfItemsResult.TargetKindsOfItems, storagesList);

#if DEBUG
                Log($"newLogicalQueryNodes = {newLogicalQueryNodes.WriteListToString()}");
#endif

                throw new NotImplementedException();
            }

            throw new NotImplementedException();
        }

        private (List<KindOfLogicalQueryNode> TargetKindsOfItems, ReplacingNotResultsStrategy ReplacingNotResultsStrategy) CalculateTargetKindsOfItems(IList<LogicalQueryNode> exceptList, ReplacingNotResultsStrategy replacingNotResultsStrategy)
        {
#if DEBUG
            Log($"exceptList = {exceptList.WriteListToString()}");
            Log($"replacingNotResultsStrategy = {replacingNotResultsStrategy}");
#endif

            throw new NotImplementedException();
        }

        private List<LogicalQueryNode> GetLogicalQueryNodes(IList<LogicalQueryNode> exceptList, ReplacingNotResultsStrategy replacingNotResultsStrategy, IList<KindOfLogicalQueryNode> targetKindsOfItems, List<StorageUsingOptions> storagesList)
        {
#if DEBUG
            Log($"exceptList = {exceptList.WriteListToString()}");
            Log($"replacingNotResultsStrategy = {replacingNotResultsStrategy}");
            Log($"targetKindsOfItems = {targetKindsOfItems.WritePODListToString()}");
#endif

            var result = new List<LogicalQueryNode>();

            foreach (var storageItem in storagesList)
            {
                //var itemsList = storageItem.Storage.LogicalStorage.GetLogicalQueryNodes(exceptList);

#if DEBUG
                //Log($"itemsList = {itemsList.WriteListToString()}");
#endif
            }

            throw new NotImplementedException();
        }
    }
}
