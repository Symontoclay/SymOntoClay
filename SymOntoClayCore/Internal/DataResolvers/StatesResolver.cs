/*MIT License

Copyright (c) 2020 - 2023 Sergiy Tolkachov

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

using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.CoreHelper.CollectionsHelpers;
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
            var dataResolversFactory = context.DataResolversFactory;

            _inheritanceResolver = dataResolversFactory.GetInheritanceResolver();
            _synonymsResolver = dataResolversFactory.GetSynonymsResolver();
        }

        private readonly InheritanceResolver _inheritanceResolver;
        private readonly SynonymsResolver _synonymsResolver;

        public StrongIdentifierValue ResolveDefaultStateName(ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return ResolveDefaultStateName(localCodeExecutionContext, _defaultOptions);
        }

        public StrongIdentifierValue ResolveDefaultStateName(ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var storage = localCodeExecutionContext.Storage;

            var storagesList = GetStoragesList(storage, KindOfStoragesList.CodeItems);

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

        public StateDef Resolve(StrongIdentifierValue name, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return Resolve(name, localCodeExecutionContext, _defaultOptions);
        }

        public StateDef Resolve(StrongIdentifierValue name, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
#if DEBUG
            //Log($"name = {name}");
#endif

            var storage = localCodeExecutionContext.Storage;

            var storagesList = GetStoragesList(storage, KindOfStoragesList.CodeItems);

#if DEBUG
            //Log($"storagesList.Count = {storagesList.Count}");
            //foreach (var tmpStorage in storagesList)
            //{
            //    Log($"tmpStorage = {tmpStorage}");
            //}
#endif

            var optionsForInheritanceResolver = options.Clone();
            optionsForInheritanceResolver.AddSelf = true;

            var weightedInheritanceItems = _inheritanceResolver.GetWeightedInheritanceItems(localCodeExecutionContext, optionsForInheritanceResolver);

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

        public List<StateDef> ResolveAllStatesList(ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return ResolveAllStatesList(localCodeExecutionContext, _defaultOptions);
        }

        public List<StateDef> ResolveAllStatesList(ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var storage = localCodeExecutionContext.Storage;

            var storagesList = GetStoragesList(storage, KindOfStoragesList.CodeItems);

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

        public List<ActivationInfoOfStateDef> ResolveActivationInfoOfStateList(ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return ResolveActivationInfoOfStateList(localCodeExecutionContext, _defaultOptions);
        }
        
        public List<ActivationInfoOfStateDef> ResolveActivationInfoOfStateList(ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var storage = localCodeExecutionContext.Storage;

            var storagesList = GetStoragesList(storage, KindOfStoragesList.CodeItems);

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

        public List<MutuallyExclusiveStatesSet> ResolveMutuallyExclusiveStatesSetsList(ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return ResolveMutuallyExclusiveStatesSetsList(localCodeExecutionContext, _defaultOptions);
        }

        public List<MutuallyExclusiveStatesSet> ResolveMutuallyExclusiveStatesSetsList(ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var storage = localCodeExecutionContext.Storage;

            var storagesList = GetStoragesList(storage, KindOfStoragesList.CodeItems);

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
            var result = NGetRawStatesList(name, storagesList, weightedInheritanceItems);

            if (result.IsNullOrEmpty())
            {
                result = GetRawStatesListFromSynonyms(name, storagesList, weightedInheritanceItems);
            }

            return result;
        }

        private List<WeightedInheritanceResultItemWithStorageInfo<StateDef>> GetRawStatesListFromSynonyms(StrongIdentifierValue name, List<StorageUsingOptions> storagesList, IList<WeightedInheritanceItem> weightedInheritanceItems)
        {
            var synonymsList = _synonymsResolver.GetSynonyms(name, storagesList);

            foreach (var synonym in synonymsList)
            {
                var rawList = NGetRawStatesList(synonym, storagesList, weightedInheritanceItems);

                if (rawList.IsNullOrEmpty())
                {
                    continue;
                }

                return rawList;
            }

            return new List<WeightedInheritanceResultItemWithStorageInfo<StateDef>>();
        }

        private List<WeightedInheritanceResultItemWithStorageInfo<StateDef>> NGetRawStatesList(StrongIdentifierValue name, List<StorageUsingOptions> storagesList, IList<WeightedInheritanceItem> weightedInheritanceItems)
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
