/*MIT License

Copyright (c) 2020 - 2021 Sergiy Tolkachov

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

using NLog;
using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.IndexedData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.DataResolvers
{
    public class ConsolidatedDataSource
    {
#if DEBUG
        //private static ILogger _gbcLogger = LogManager.GetCurrentClassLogger();
#endif

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

        public IList<LogicalQueryNode> AllRelationsForProductions
        {
            get
            {
                lock (_lockObj)
                {
                    var result = new List<LogicalQueryNode>();

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

        public IList<BaseRulePart> GetIndexedRulePartOfFactsByKeyOfRelation(StrongIdentifierValue key)
        {
            lock (_lockObj)
            {
#if DEBUG
                //DebugLogger.Instance.Info($"key = {key}");
#endif

                var initialResult = new List<BaseRulePart>();

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

                var result = new List<BaseRulePart>();

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

        public IList<BaseRulePart> GetIndexedRulePartWithOneRelationWithVarsByKeyOfRelation(StrongIdentifierValue name)
        {
            lock (_lockObj)
            {
#if DEBUG
                //_gbcLogger.Info($"name = {name}");
#endif

                var initialResult = new List<BaseRulePart>();

                var dataSourcesSettingsOrderedByPriorityAndUseProductionsList = _dataSourcesSettingsOrderedByPriorityAndUseProductionsList;

                foreach (var dataSourcesSettings in dataSourcesSettingsOrderedByPriorityAndUseProductionsList)
                {
                    var indexedRulePartWithOneRelationsList = dataSourcesSettings.Storage.LogicalStorage.GetIndexedRulePartWithOneRelationWithVarsByKeyOfRelation(name);

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

                var result = new List<BaseRulePart>();

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
