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

using SymOntoClay.Common.CollectionsHelpers;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Monitor.Common;
using System.Collections.Generic;
using System.Linq;

namespace SymOntoClay.Core.Internal.DataResolvers
{
    public class ConsolidatedDataSource
    {
        public ConsolidatedDataSource(List<StorageUsingOptions> storagesList)
        {
            _storagesList = storagesList;

            _dataSourcesSettingsOrderedByPriorityList = storagesList.OrderByDescending(p => p.Priority).ToList();
            _dataSourcesSettingsOrderedByPriorityAndUseFactsList = _dataSourcesSettingsOrderedByPriorityList.Where(p => p.UseFacts).ToList();
            _dataSourcesSettingsOrderedByPriorityAndUseProductionsList = _dataSourcesSettingsOrderedByPriorityList.Where(p => p.UseProductions).ToList();
            _dataSourcesSettingsOrderedByPriorityAndUseInheritanceFacts = _dataSourcesSettingsOrderedByPriorityList.Where(p => p.UseInheritanceFacts).ToList();
        }

        private readonly object _lockObj = new object();
        private readonly List<StorageUsingOptions> _storagesList;
        private readonly IList<StorageUsingOptions> _dataSourcesSettingsOrderedByPriorityList;
        private readonly IList<StorageUsingOptions> _dataSourcesSettingsOrderedByPriorityAndUseFactsList;
        private readonly IList<StorageUsingOptions> _dataSourcesSettingsOrderedByPriorityAndUseProductionsList;
        private readonly IList<StorageUsingOptions> _dataSourcesSettingsOrderedByPriorityAndUseInheritanceFacts;

        private static StrongIdentifierValue _isRelationName = NameHelper.CreateName("is");

        public List<StorageUsingOptions> StoragesList => _storagesList;

        public IList<LogicalQueryNode> AllRelationsForProductions(IMonitorLogger logger, ILogicalSearchStorageContext logicalSearchStorageContext, LogicalSearchExplainNode parentExplainNode, LogicalSearchExplainNode rootParentExplainNode)
        {
            lock (_lockObj)
            {
                LogicalSearchExplainNode currentExplainNode = null;

                if (parentExplainNode != null)
                {
                    currentExplainNode = new LogicalSearchExplainNode(rootParentExplainNode)
                    {
                        Kind = KindOfLogicalSearchExplainNode.ConsolidatedDataSource
                    };

                    LogicalSearchExplainNode.LinkNodes(parentExplainNode, currentExplainNode);
                }

                var result = new List<LogicalQueryNode>();

                var dataSourcesSettingsOrderedByPriorityAndUseProductionsList = _dataSourcesSettingsOrderedByPriorityAndUseProductionsList;

                foreach (var dataSourcesSettings in dataSourcesSettingsOrderedByPriorityAndUseProductionsList)
                {
                    LogicalSearchExplainNode localResultExplainNode = null;

                    if (currentExplainNode != null)
                    {
                        localResultExplainNode = new LogicalSearchExplainNode(rootParentExplainNode)
                        {
                            Kind = KindOfLogicalSearchExplainNode.DataSourceResult
                        };

                        LogicalSearchExplainNode.LinkNodes(currentExplainNode, localResultExplainNode);
                    }

                    var targetRelationsList = dataSourcesSettings.Storage.LogicalStorage.GetAllRelations(logger, logicalSearchStorageContext, localResultExplainNode, rootParentExplainNode);

                    if (localResultExplainNode != null)
                    {
                        localResultExplainNode.RelationsList = targetRelationsList;
                    }

                    if (targetRelationsList.IsNullOrEmpty())
                    {
                        continue;
                    }

                    result.AddRange(targetRelationsList);
                }

#if DEBUG







#endif

                return result;
            }
        }

        public IList<BaseRulePart> GetIndexedRulePartOfFactsByKeyOfRelation(IMonitorLogger logger, StrongIdentifierValue name, ILogicalSearchStorageContext logicalSearchStorageContext, LogicalSearchExplainNode parentExplainNode, LogicalSearchExplainNode rootParentExplainNode)
        {
            lock (_lockObj)
            {
                LogicalSearchExplainNode currentExplainNode = null;

                if (parentExplainNode != null)
                {
                    currentExplainNode = new LogicalSearchExplainNode(rootParentExplainNode)
                    {
                        Kind = KindOfLogicalSearchExplainNode.ConsolidatedDataSource,
                        Key = name
                    };

                    LogicalSearchExplainNode.LinkNodes(parentExplainNode, currentExplainNode);
                }

                var initialResult = new List<BaseRulePart>();

                IList<StorageUsingOptions> targetSourcesList = null;

                if(name == _isRelationName)
                {
                    targetSourcesList = _dataSourcesSettingsOrderedByPriorityAndUseInheritanceFacts;
                }
                else
                {
                    targetSourcesList = _dataSourcesSettingsOrderedByPriorityAndUseFactsList;
                }

                foreach (var dataSourcesSettings in targetSourcesList)
                {
                    LogicalSearchExplainNode localResultExplainNode = null;

                    if (currentExplainNode != null)
                    {
                        localResultExplainNode = new LogicalSearchExplainNode(rootParentExplainNode)
                        {
                            Kind = KindOfLogicalSearchExplainNode.DataSourceResult
                        };

                        LogicalSearchExplainNode.LinkNodes(currentExplainNode, localResultExplainNode);
                    }

                    var rulePartsOfFactsList = dataSourcesSettings.Storage.LogicalStorage.GetIndexedRulePartOfFactsByKeyOfRelation(logger, name, logicalSearchStorageContext, localResultExplainNode, rootParentExplainNode);

                    if(localResultExplainNode != null)
                    {
                        localResultExplainNode.BaseRulePartList = rulePartsOfFactsList;
                    }

                    if (rulePartsOfFactsList.IsNullOrEmpty())
                    {
                        continue;
                    }

                    initialResult.AddRange(rulePartsOfFactsList);
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

                return result;
            }
        }

        public IList<BaseRulePart> GetIndexedRulePartWithOneRelationWithVarsByKeyOfRelation(IMonitorLogger logger, StrongIdentifierValue name, ILogicalSearchStorageContext logicalSearchStorageContext, LogicalSearchExplainNode parentExplainNode, LogicalSearchExplainNode rootParentExplainNode)
        {
            lock (_lockObj)
            {
                LogicalSearchExplainNode currentExplainNode = null;

                if (parentExplainNode != null)
                {
                    currentExplainNode = new LogicalSearchExplainNode(rootParentExplainNode)
                    {
                        Kind = KindOfLogicalSearchExplainNode.ConsolidatedDataSource,
                        Key = name
                    };

                    LogicalSearchExplainNode.LinkNodes(parentExplainNode, currentExplainNode);
                }

                var initialResult = new List<BaseRulePart>();

                IList<StorageUsingOptions> targetSourcesList = null;

                if (name == _isRelationName)
                {
                    targetSourcesList = _dataSourcesSettingsOrderedByPriorityAndUseInheritanceFacts;
                }
                else
                {
                    targetSourcesList = _dataSourcesSettingsOrderedByPriorityAndUseProductionsList;
                }               

                foreach (var dataSourcesSettings in targetSourcesList)
                {
                    LogicalSearchExplainNode localResultExplainNode = null;

                    if (currentExplainNode != null)
                    {
                        localResultExplainNode = new LogicalSearchExplainNode(rootParentExplainNode)
                        {
                            Kind = KindOfLogicalSearchExplainNode.DataSourceResult
                        };

                        LogicalSearchExplainNode.LinkNodes(currentExplainNode, localResultExplainNode);
                    }

                    var rulePartWithOneRelationsList = dataSourcesSettings.Storage.LogicalStorage.GetIndexedRulePartWithOneRelationWithVarsByKeyOfRelation(logger, name, logicalSearchStorageContext, localResultExplainNode, rootParentExplainNode);

                    if (localResultExplainNode != null)
                    {
                        localResultExplainNode.BaseRulePartList = rulePartWithOneRelationsList;
                    }

                    if (rulePartWithOneRelationsList.IsNullOrEmpty())
                    {
                        continue;
                    }

                    initialResult.AddRange(rulePartWithOneRelationsList);
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

                return result;
            }
        }
    }
}
