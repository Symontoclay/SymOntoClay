using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.Core.Internal.Instances;
using SymOntoClay.Monitor.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SymOntoClay.Core.Internal.DataResolvers
{
    public class PropertiesResolver : BaseResolver
    {
        public PropertiesResolver(IMainStorageContext context)
            : base(context)
        {
        }

        public ResolverOptions DefaultOptions { get; private set; } = ResolverOptions.GetDefaultOptions();

        public PropertyInstance Resolve(IMonitorLogger logger, StrongIdentifierValue propertyName, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return Resolve(logger, propertyName, localCodeExecutionContext, DefaultOptions);
        }

        public PropertyInstance Resolve(IMonitorLogger logger, StrongIdentifierValue propertyName, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
#if DEBUG
            //Info("F003D1F2-A299-411F-932C-7C226A0D13CC", $"propertyName = {propertyName}");
#endif

            var storagesList = GetStoragesList(logger, localCodeExecutionContext.Storage, KindOfStoragesList.Property);

#if DEBUG
            //Info("C6A2F71F-4B86-4418-B806-82114687C6B4", $"storagesList.Count = {storagesList.Count}");
#endif

            var optionsForInheritanceResolver = options.Clone();
            optionsForInheritanceResolver.AddSelf = true;

            var weightedInheritanceItems = _inheritanceResolver.GetWeightedInheritanceItems(logger, localCodeExecutionContext, optionsForInheritanceResolver);

#if DEBUG
            //Info("35C3DF0D-8B07-4F45-8741-1526A7AE5C64", $"weightedInheritanceItems.Count = {weightedInheritanceItems.Count}");
#endif

            var rawList = GetRawPropertiesList(logger, propertyName, storagesList, weightedInheritanceItems);

#if DEBUG
            //Info("971B9D45-54A7-4D6C-8939-66FCE52F227D", $"rawList.Count = {rawList.Count}");
#endif

            if (!rawList.Any())
            {
                return null;
            }

            var filteredList = FilterCodeItems(logger, rawList, localCodeExecutionContext.Holder, localCodeExecutionContext);

#if DEBUG
            //Info("A5B248B1-2917-4940-9B9A-37DF79146444", $"filteredList.Count = {filteredList.Count}");
#endif

            if (!filteredList.Any())
            {
                return null;
            }

            if (filteredList.Count == 1)
            {
                return filteredList.Single().ResultItem;
            }

            var minStorageDistance = filteredList.Min(p => p.StorageDistance);

            filteredList = filteredList.Where(p => p.StorageDistance == minStorageDistance).ToList();

            if (filteredList.Count == 1)
            {
                return filteredList.Single().ResultItem;
            }

            return OrderAndDistinctByInheritance(logger, filteredList, options).FirstOrDefault()?.ResultItem;
        }

        private List<WeightedInheritanceResultItemWithStorageInfo<PropertyInstance>> GetRawPropertiesList(IMonitorLogger logger, StrongIdentifierValue name, List<StorageUsingOptions> storagesList, IList<WeightedInheritanceItem> weightedInheritanceItems)
        {
            if (!storagesList.Any())
            {
                return new List<WeightedInheritanceResultItemWithStorageInfo<PropertyInstance>>();
            }

            var result = new List<WeightedInheritanceResultItemWithStorageInfo<PropertyInstance>>();

            foreach (var storageItem in storagesList)
            {
                var itemsList = storageItem.Storage.PropertyStorage.GetPropertyDirectly(logger, name, weightedInheritanceItems);

                if (!itemsList.Any())
                {
                    continue;
                }

                var distance = storageItem.Priority;
                var storage = storageItem.Storage;

                foreach (var item in itemsList)
                {
                    result.Add(new WeightedInheritanceResultItemWithStorageInfo<PropertyInstance>(item, distance, storage));
                }
            }

            return result;
        }
    }
}
