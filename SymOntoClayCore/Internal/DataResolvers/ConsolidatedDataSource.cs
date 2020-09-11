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

                    return result;
                }
            }
        }
    }
}
