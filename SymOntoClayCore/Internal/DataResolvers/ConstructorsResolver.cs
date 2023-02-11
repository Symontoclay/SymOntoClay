using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace SymOntoClay.Core.Internal.DataResolvers
{
    public class ConstructorsResolver : BaseMethodsResolver
    {
        public ConstructorsResolver(IMainStorageContext context)
            : base(context)
        {
        }

        private readonly List<Constructor> _emptyConstructorsList = new List<Constructor>();

        public Constructor ResolveOnlyOwn(StrongIdentifierValue holder, LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
#if DEBUG
            Log($"holder = {holder}");
#endif

            var storagesList = GetStoragesList(localCodeExecutionContext.Storage, KindOfStoragesList.CodeItems);

#if DEBUG
            //Log($"storagesList.Count = {storagesList.Count}");
            //foreach (var tmpStorage in storagesList)
            //{
            //    Log($"tmpStorage.Key = {tmpStorage.Key}; tmpStorage.Value.Kind = '{tmpStorage.Value.Kind}'");
            //}
#endif

            var rawList = GetRawList(0, storagesList, new List<WeightedInheritanceItem>() { InheritanceResolver.GetSelfWeightedInheritanceItem(holder) });

#if DEBUG
            Log($"rawList = {rawList.WriteListToString()}");
#endif

            if (!rawList.Any())
            {
                return null;
            }

            var filteredList = Filter(rawList);

#if DEBUG
            Log($"filteredList = {filteredList.WriteListToString()}");
#endif

            throw new NotImplementedException();
        }

        public Constructor ResolveOnlyOwn(StrongIdentifierValue holder, Dictionary<StrongIdentifierValue, Value> namedParameters, LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
#if DEBUG
            //Log($"holder = {holder}");
            //Log($"namedParameters = {namedParameters.WriteDict_1_ToString()}");
#endif

            var storagesList = GetStoragesList(localCodeExecutionContext.Storage, KindOfStoragesList.CodeItems);

#if DEBUG
            //Log($"storagesList.Count = {storagesList.Count}");
            //foreach (var tmpStorage in storagesList)
            //{
            //    Log($"tmpStorage.Key = {tmpStorage.Key}; tmpStorage.Value.Kind = '{tmpStorage.Value.Kind}'");
            //}
#endif

            var rawList = GetRawList(namedParameters.Count, storagesList, new List<WeightedInheritanceItem>() { InheritanceResolver.GetSelfWeightedInheritanceItem(holder) });

#if DEBUG
            //Log($"rawList = {rawList.WriteListToString()}");
#endif

            if (!rawList.Any())
            {
                return null;
            }

            var filteredList = Filter(rawList);

#if DEBUG
            //Log($"filteredList = {filteredList.WriteListToString()}");
#endif

            if (!filteredList.Any())
            {
                return null;
            }

            filteredList = FilterByTypeOfParameters(filteredList, namedParameters, localCodeExecutionContext, options);

#if DEBUG
            //Log($"filteredList (2) = {filteredList.WriteListToString()}");
#endif

            if (!filteredList.Any())
            {
                return null;
            }

            if (filteredList.Count == 1)
            {
                return filteredList.Single().ResultItem;
            }

            return GetTargetValueFromList(filteredList, namedParameters.Count, localCodeExecutionContext, options);
        }

        public Constructor ResolveOnlyOwn(StrongIdentifierValue holder, List<Value> positionedParameters, LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
#if DEBUG
            //Log($"holder = {holder}");
            //Log($"positionedParameters = {positionedParameters.WriteListToString()}");
#endif

            var storagesList = GetStoragesList(localCodeExecutionContext.Storage, KindOfStoragesList.CodeItems);

#if DEBUG
            //Log($"storagesList.Count = {storagesList.Count}");
            //foreach (var tmpStorage in storagesList)
            //{
            //    Log($"tmpStorage.Key = {tmpStorage.Key}; tmpStorage.Value.Kind = '{tmpStorage.Value.Kind}'");
            //}
#endif

            var rawList = GetRawList(positionedParameters.Count, storagesList, new List<WeightedInheritanceItem>() { InheritanceResolver.GetSelfWeightedInheritanceItem(holder) });

#if DEBUG
            //Log($"rawList = {rawList.WriteListToString()}");
#endif

            if (!rawList.Any())
            {
                return null;
            }

            var filteredList = Filter(rawList);

#if DEBUG
            //Log($"filteredList = {filteredList.WriteListToString()}");
#endif

            if (!filteredList.Any())
            {
                return null;
            }

            filteredList = FilterByTypeOfParameters(filteredList, positionedParameters, localCodeExecutionContext, options);

#if DEBUG
            //Log($"filteredList (2) = {filteredList.WriteListToString()}");
#endif

            if (!filteredList.Any())
            {
                return null;
            }

            if (filteredList.Count == 1)
            {
                return filteredList.Single().ResultItem;
            }

            return GetTargetValueFromList(filteredList, positionedParameters.Count, localCodeExecutionContext, options);
        }

        public List<Constructor> ResolveListWithSelfAndDirectInheritance(StrongIdentifierValue holder, LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
#if DEBUG
            //Log($"holder = {holder}");
#endif

            var storagesList = GetStoragesList(localCodeExecutionContext.Storage, KindOfStoragesList.CodeItems);

#if DEBUG
            //Log($"storagesList.Count = {storagesList.Count}");
            //foreach (var tmpStorage in storagesList)
            //{
            //    Log($"tmpStorage.Key = {tmpStorage.Key}; tmpStorage.Value.Kind = '{tmpStorage.Value.Kind}'");
            //}
#endif

            var optionsForInheritanceResolver = options.Clone();
            optionsForInheritanceResolver.AddSelf = false;
            optionsForInheritanceResolver.AddTopType = false;
            optionsForInheritanceResolver.OnlyDirectInheritance= true;

            var weightedInheritanceItems = _inheritanceResolver.GetWeightedInheritanceItems(holder, localCodeExecutionContext, optionsForInheritanceResolver);

#if DEBUG
            //Log($"weightedInheritanceItems = {weightedInheritanceItems.WriteListToString()}");
#endif

            var rawList = GetRawList(0, storagesList, weightedInheritanceItems);

#if DEBUG
            //Log($"rawList = {rawList.WriteListToString()}");
#endif

            if (!rawList.Any())
            {
                return _emptyConstructorsList;
            }

            var filteredList = Filter(rawList);

#if DEBUG
            //Log($"filteredList = {filteredList.WriteListToString()}");
#endif

            if (!filteredList.Any())
            {
                return _emptyConstructorsList;
            }

            return filteredList.Select(p => p.ResultItem).ToList();
        }

        private List<WeightedInheritanceResultItemWithStorageInfo<Constructor>>  GetRawList(int paramsCount, List<StorageUsingOptions> storagesList, IList<WeightedInheritanceItem> weightedInheritanceItems)
        {
#if DEBUG
            //Log($"paramsCount = {paramsCount}");
#endif

            var result = new List<WeightedInheritanceResultItemWithStorageInfo<Constructor>>();

            foreach (var storageItem in storagesList)
            {
                var itemsList = storageItem.Storage.ConstructorsStorage.GetConstructorsDirectly(paramsCount, weightedInheritanceItems);

#if DEBUG
                //Log($"itemsList = {itemsList?.WriteListToString()}");
#endif

                if (!itemsList.Any())
                {
                    continue;
                }

                var distance = storageItem.Priority;
                var storage = storageItem.Storage;

                foreach (var item in itemsList)
                {
                    result.Add(new WeightedInheritanceResultItemWithStorageInfo<Constructor>(item, distance, storage));
                }
            }

            return result;
        }

        public List<Constructor> ResolvePreConstructors(StrongIdentifierValue holder, LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
#if DEBUG
            //Log($"holder = {holder}");
#endif

            var storagesList = GetStoragesList(localCodeExecutionContext.Storage, KindOfStoragesList.CodeItems);

#if DEBUG
            //Log($"storagesList.Count = {storagesList.Count}");
            //foreach (var tmpStorage in storagesList)
            //{
            //    Log($"tmpStorage.Key = {tmpStorage.Key}; tmpStorage.Value.Kind = '{tmpStorage.Value.Kind}'");
            //}
#endif

            var optionsForInheritanceResolver = options.Clone();
            optionsForInheritanceResolver.AddSelf = true;

            var weightedInheritanceItems = _inheritanceResolver.GetWeightedInheritanceItems(holder, localCodeExecutionContext, optionsForInheritanceResolver);

#if DEBUG
            //Log($"weightedInheritanceItems = {weightedInheritanceItems.WriteListToString()}");
#endif

            var rawList = GetRawPreConstructorsList(storagesList, weightedInheritanceItems);

#if DEBUG
            //Log($"rawList = {rawList.WriteListToString()}");
#endif

            if (!rawList.Any())
            {
                return _emptyConstructorsList;
            }

            return rawList.OrderByDescending(p => p.Distance).Select(p => p.ResultItem).ToList();
        }

        private List<WeightedInheritanceResultItemWithStorageInfo<Constructor>> GetRawPreConstructorsList(List<StorageUsingOptions> storagesList, IList<WeightedInheritanceItem> weightedInheritanceItems)
        {
            var result = new List<WeightedInheritanceResultItemWithStorageInfo<Constructor>>();

            foreach (var storageItem in storagesList)
            {
                var itemsList = storageItem.Storage.ConstructorsStorage.GetPreConstructorsDirectly(weightedInheritanceItems);

#if DEBUG
                //Log($"itemsList = {itemsList?.WriteListToString()}");
#endif

                if (!itemsList.Any())
                {
                    continue;
                }

                var distance = storageItem.Priority;
                var storage = storageItem.Storage;

                foreach (var item in itemsList)
                {
                    result.Add(new WeightedInheritanceResultItemWithStorageInfo<Constructor>(item, distance, storage));
                }
            }

            return result;
        }
    }
}
