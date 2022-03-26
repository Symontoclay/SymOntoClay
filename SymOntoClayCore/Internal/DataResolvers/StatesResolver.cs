using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.DataResolvers
{
    public class StatesResolver : BaseResolver
    {
        public StatesResolver(IMainStorageContext context)
            : base(context)
        {
        }

        public StrongIdentifierValue ResolveDefaultStateName(LocalCodeExecutionContext localCodeExecutionContext)
        {
            return ResolveDefaultStateName(localCodeExecutionContext, _defaultOptions);
        }

        public StrongIdentifierValue ResolveDefaultStateName(LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var storage = localCodeExecutionContext.Storage;

            var storagesList = GetStoragesList(storage);

#if DEBUG
            //Log($"storagesList.Count = {storagesList.Count}");
            //foreach (var tmpStorage in storagesList)
            //{
            //    Log($"tmpStorage = {tmpStorage}");
            //}
#endif

            if (!storagesList.Any())
            {
                return null;
            }

            foreach (var storageItem in storagesList)
            {
#if DEBUG
                //Log($"storageItem = {storageItem}");
#endif

                var item = storageItem.Storage.StatesStorage.GetDefaultStateNameDirectly();

#if DEBUG
                //Log($"item = {item}");
#endif

                if(item != null)
                {
                    return item;
                }
            }

            return null;
        }

        public StateDef Resolve(StrongIdentifierValue name, LocalCodeExecutionContext localCodeExecutionContext)
        {
            return Resolve(name, localCodeExecutionContext, _defaultOptions);
        }

        public StateDef Resolve(StrongIdentifierValue name, LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
#if DEBUG
            //Log($"name = {name}");
#endif

            var storage = localCodeExecutionContext.Storage;

            var storagesList = GetStoragesList(storage);

#if DEBUG
            //Log($"storagesList.Count = {storagesList.Count}");
            //foreach (var tmpStorage in storagesList)
            //{
            //    Log($"tmpStorage = {tmpStorage}");
            //}
#endif

            var inheritanceResolver = _context.DataResolversFactory.GetInheritanceResolver();

            var optionsForInheritanceResolver = options.Clone();
            optionsForInheritanceResolver.AddSelf = true;

            var weightedInheritanceItems = inheritanceResolver.GetWeightedInheritanceItems(localCodeExecutionContext, optionsForInheritanceResolver);

#if DEBUG
            //Log($"weightedInheritanceItems = {weightedInheritanceItems.WriteListToString()}");
#endif

            var rawList = GetRawStatesList(name, storagesList, weightedInheritanceItems);

#if DEBUG
            //Log($"rawList = {rawList.WriteListToString()}");
#endif

            if (!rawList.Any())
            {
                return null;
            }

            var filteredList = FilterCodeItems(rawList, localCodeExecutionContext);

#if DEBUG
            //Log($"filteredList = {filteredList.WriteListToString()}");
#endif

            if (!filteredList.Any())
            {
                return null;
            }

            if (filteredList.Count == 1)
            {
                return filteredList.Single().ResultItem;
            }

            return OrderAndDistinctByInheritance(filteredList, options).FirstOrDefault()?.ResultItem;
        }

        public List<StateDef> ResolveAllStatesList(LocalCodeExecutionContext localCodeExecutionContext)
        {
            return ResolveAllStatesList(localCodeExecutionContext, _defaultOptions);
        }

        public List<StateDef> ResolveAllStatesList(LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var storage = localCodeExecutionContext.Storage;

            var storagesList = GetStoragesList(storage);

            if (!storagesList.Any())
            {
                return new List<StateDef>();
            }

            var result = new List<StateDef>();

            foreach (var storageItem in storagesList)
            {
#if DEBUG
                //Log($"storageItem = {storageItem}");
#endif

                var itemsList = storageItem.Storage.StatesStorage.GetAllStatesListDirectly();

#if DEBUG
                //Log($"itemsList = {itemsList.WriteListToString()}");
#endif

                result.AddRange(itemsList);
            }

            return result;
        }

        public List<ActivationInfoOfStateDef> ResolveActivationInfoOfStateList(LocalCodeExecutionContext localCodeExecutionContext)
        {
            return ResolveActivationInfoOfStateList(localCodeExecutionContext, _defaultOptions);
        }
        
        public List<ActivationInfoOfStateDef> ResolveActivationInfoOfStateList(LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var storage = localCodeExecutionContext.Storage;

            var storagesList = GetStoragesList(storage);

#if DEBUG
            //Log($"storagesList.Count = {storagesList.Count}");
            //foreach (var tmpStorage in storagesList)
            //{
            //    Log($"tmpStorage = {tmpStorage}");
            //}
#endif

            if (!storagesList.Any())
            {
                return new List<ActivationInfoOfStateDef>();
            }

            var result = new List<ActivationInfoOfStateDef>();

            foreach (var storageItem in storagesList)
            {
#if DEBUG
                //Log($"storageItem = {storageItem}");
#endif

                var itemsList = storageItem.Storage.StatesStorage.GetActivationInfoOfStateListDirectly();

#if DEBUG
                //Log($"itemsList = {itemsList.WriteListToString()}");
#endif

                result.AddRange(itemsList);
            }

            return result;
        }

        public List<MutuallyExclusiveStatesSet> ResolveMutuallyExclusiveStatesSetsList(LocalCodeExecutionContext localCodeExecutionContext)
        {
            return ResolveMutuallyExclusiveStatesSetsList(localCodeExecutionContext, _defaultOptions);
        }

        public List<MutuallyExclusiveStatesSet> ResolveMutuallyExclusiveStatesSetsList(LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var storage = localCodeExecutionContext.Storage;

            var storagesList = GetStoragesList(storage);

#if DEBUG
            //Log($"storagesList.Count = {storagesList.Count}");
            //foreach (var tmpStorage in storagesList)
            //{
            //    Log($"tmpStorage = {tmpStorage}");
            //}
#endif

            if (!storagesList.Any())
            {
                return new List<MutuallyExclusiveStatesSet>();
            }

            var result = new List<MutuallyExclusiveStatesSet>();

            foreach (var storageItem in storagesList)
            {
#if DEBUG
                //Log($"storageItem = {storageItem}");
#endif

                var itemsList = storageItem.Storage.StatesStorage.GetMutuallyExclusiveStatesSetsListDirectly();

#if DEBUG
                //Log($"itemsList = {itemsList.WriteListToString()}");
#endif

                result.AddRange(itemsList);
            }

            return result;
        }

        private List<WeightedInheritanceResultItemWithStorageInfo<StateDef>> GetRawStatesList(StrongIdentifierValue name, List<StorageUsingOptions> storagesList, IList<WeightedInheritanceItem> weightedInheritanceItems)
        {
#if DEBUG
            //Log($"name = {name}");
#endif

            if (!storagesList.Any())
            {
                return new List<WeightedInheritanceResultItemWithStorageInfo<StateDef>>();
            }

            var result = new List<WeightedInheritanceResultItemWithStorageInfo<StateDef>>();

            foreach (var storageItem in storagesList)
            {
#if DEBUG
                //Log($"storageItem = {storageItem}");
#endif

                var itemsList = storageItem.Storage.StatesStorage.GetStatesDirectly(name, weightedInheritanceItems);

#if DEBUG
                //Log($"itemsList = {itemsList.WriteListToString()}");
#endif

                if (!itemsList.Any())
                {
                    continue;
                }

                var distance = storageItem.Priority;
                var storage = storageItem.Storage;

                foreach (var item in itemsList)
                {
                    result.Add(new WeightedInheritanceResultItemWithStorageInfo<StateDef>(item, distance, storage));
                }
            }

            return result;
        }

        private readonly ResolverOptions _defaultOptions = ResolverOptions.GetDefaultOptions();
    }
}
