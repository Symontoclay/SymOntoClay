using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.IndexedData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.DataResolvers
{
    public class ConsolidatedDataSource
    {
        public ConsolidatedDataSource(List<StorageUsingOptions> storagesList)
        {
            _dataSourcesSettingsOrderedByPriorityList = storagesList.OrderByDescending(p => p.Priority).ToList();
            _dataSourcesSettingsOrderedByPriorityAndUseFactsList = _dataSourcesSettingsOrderedByPriorityList.Where(p => p.UseFacts).ToList();
            _dataSourcesSettingsOrderedByPriorityAndUseProductionsList = _dataSourcesSettingsOrderedByPriorityList.Where(p => p.UseProductions).ToList();
            _dataSourcesSettingsOrderedByPriorityAndUseAdditionalInstances = _dataSourcesSettingsOrderedByPriorityList.Where(p => p.UseAdditionalInstances).ToList();
        }

        private readonly object _lockObj = new object();
        private IList<StorageUsingOptions> _dataSourcesSettingsOrderedByPriorityList;
        private IList<StorageUsingOptions> _dataSourcesSettingsOrderedByPriorityAndUseFactsList;
        private IList<StorageUsingOptions> _dataSourcesSettingsOrderedByPriorityAndUseProductionsList;
        private IList<StorageUsingOptions> _dataSourcesSettingsOrderedByPriorityAndUseAdditionalInstances;

        public IList<RelationIndexedLogicalQueryNode> AllRelationsForProductions
        {
            get
            {
                lock (_lockObj)
                {
                    var result = new List<RelationIndexedLogicalQueryNode>();

                    var dataSourcesSettingsOrderedByPriorityAndUseProductionsList = _dataSourcesSettingsOrderedByPriorityAndUseProductionsList;

                    foreach (var dataSourcesSettings in dataSourcesSettingsOrderedByPriorityAndUseProductionsList)
                    {
                        var targetRelationsList = dataSourcesSettings.Storage.LogicalStorage.GetAllRelations();

                        if (targetRelationsList == null)
                        {
                            continue;
                        }

                        result.AddRange(targetRelationsList);
                    }

#if DEBUG
                    //var tmpList = result.GroupBy(p => p.GetLongHashCode()).ToDictionary(p => p.Key, p => p.ToList());

                    //DebugLogger.Instance.Info($"tmpList.Count = {tmpList.Count}");

                    //foreach (var tmpKVPItem in tmpList)
                    //{
                    //    DebugLogger.Instance.Info($"tmpKVPItem.Key = {tmpKVPItem.Key}");
                    //    DebugLogger.Instance.Info($"tmpKVPItem.Value.Count = {tmpKVPItem.Value.Count}");

                    //    if(tmpKVPItem.Value.Count > 1)
                    //    {
                    //        throw new NotImplementedException();
                    //    }
                    //}

                    //var tmpList2 = result.GroupBy(p => p.Key).ToDictionary(p => p.Key, p => p.ToList());

                    //DebugLogger.Instance.Info($"tmpList2.Count = {tmpList2.Count}");

                    //foreach (var tmpKVPItem in tmpList2)
                    //{
                    //    DebugLogger.Instance.Info($"tmpKVPItem.Key (2) = {tmpKVPItem.Key}");
                    //    DebugLogger.Instance.Info($"tmpKVPItem.Value.Count (2) = {tmpKVPItem.Value.Count}");

                    //    if (tmpKVPItem.Value.Count > 1)
                    //    {
                    //        throw new NotImplementedException();
                    //    }
                    //}
#endif

                    return result;
                }
            }
        }

        public IList<IndexedBaseRulePart> GetIndexedRulePartOfFactsByKeyOfRelation(ulong key)
        {
            lock (_lockObj)
            {
#if DEBUG
                //DebugLogger.Instance.Info($"key = {key}");
#endif

                var initialResult = new List<IndexedBaseRulePart>();

                var dataSourcesSettingsOrderedByPriorityList = _dataSourcesSettingsOrderedByPriorityAndUseFactsList;

                foreach (var dataSourcesSettings in dataSourcesSettingsOrderedByPriorityList)
                {
                    var indexedRulePartsOfFactsList = dataSourcesSettings.Storage.LogicalStorage.GetIndexedRulePartOfFactsByKeyOfRelation(key);

                    if (indexedRulePartsOfFactsList == null)
                    {
                        continue;
                    }

                    initialResult.AddRange(indexedRulePartsOfFactsList);
                }

                if (initialResult.Count <= 1)
                {
                    return initialResult;
                }

                var result = new List<IndexedBaseRulePart>();

                var groupedDict = initialResult.GroupBy(p => p.GetLongHashCode()).ToDictionary(p => p.Key, p => p.ToList());

                foreach (var kvpItem in groupedDict)
                {
                    result.Add(kvpItem.Value.First());
                }

#if DEBUG
                //var tmpList = initialResult.GroupBy(p => p.GetLongHashCode()).ToDictionary(p => p.Key, p => p.ToList());

                //DebugLogger.Instance.Info($"initialResult.Count = {initialResult.Count}");
                //DebugLogger.Instance.Info($"result.Count = {result.Count}");
                //DebugLogger.Instance.Info($"tmpList.Count = {tmpList.Count}");

                //foreach (var tmpKVPItem in tmpList)
                //{
                //    DebugLogger.Instance.Info($"tmpKVPItem.Key = {tmpKVPItem.Key}");
                //    DebugLogger.Instance.Info($"tmpKVPItem.Value.Count = {tmpKVPItem.Value.Count}");

                //    if (tmpKVPItem.Value.Count > 1)
                //    {
                //        foreach (var tmpValueItem in tmpKVPItem.Value)
                //        {
                //            DebugLogger.Instance.Info(DebugHelperForRuleInstance.BaseRulePartToString(tmpValueItem.OriginRulePart));
                //        }

                //        throw new NotImplementedException();
                //    }
                //}
#endif

                return result;
            }
        }

        public IList<IndexedBaseRulePart> GetIndexedRulePartWithOneRelationWithVarsByKeyOfRelation(ulong key)
        {
            lock (_lockObj)
            {
#if DEBUG
                //DebugLogger.Instance.Info($"key = {key}");
#endif

                var initialResult = new List<IndexedBaseRulePart>();

                var dataSourcesSettingsOrderedByPriorityAndUseProductionsList = _dataSourcesSettingsOrderedByPriorityAndUseProductionsList;

                foreach (var dataSourcesSettings in dataSourcesSettingsOrderedByPriorityAndUseProductionsList)
                {
                    var indexedRulePartWithOneRelationsList = dataSourcesSettings.Storage.LogicalStorage.GetIndexedRulePartWithOneRelationWithVarsByKeyOfRelation(key);

                    if (indexedRulePartWithOneRelationsList == null)
                    {
                        continue;
                    }

                    initialResult.AddRange(indexedRulePartWithOneRelationsList);
                }

                if(initialResult.Count <= 1)
                {
                    return initialResult;
                }

                var result = new List<IndexedBaseRulePart>();

                var groupedDict = initialResult.GroupBy(p => p.GetLongHashCode()).ToDictionary(p => p.Key, p => p.ToList());

                foreach(var kvpItem in groupedDict)
                {
                    result.Add(kvpItem.Value.First());
                }

#if DEBUG
                //var tmpList = initialResult.GroupBy(p => p.GetLongHashCode()).ToDictionary(p => p.Key, p => p.ToList());

                //DebugLogger.Instance.Info($"initialResult.Count = {initialResult.Count}");
                //DebugLogger.Instance.Info($"result.Count = {result.Count}");
                //DebugLogger.Instance.Info($"tmpList.Count = {tmpList.Count}");

                //foreach (var tmpKVPItem in tmpList)
                //{
                //    DebugLogger.Instance.Info($"tmpKVPItem.Key = {tmpKVPItem.Key}");
                //    DebugLogger.Instance.Info($"tmpKVPItem.Value.Count = {tmpKVPItem.Value.Count}");

                //    if (tmpKVPItem.Value.Count > 1)
                //    {
                //        foreach(var tmpValueItem in tmpKVPItem.Value)
                //        {
                //            DebugLogger.Instance.Info(DebugHelperForRuleInstance.BaseRulePartToString(tmpValueItem.OriginRulePart));
                //        }

                //        throw new NotImplementedException();
                //    }
                //}
#endif

                return result;
            }
        }
    }
}
