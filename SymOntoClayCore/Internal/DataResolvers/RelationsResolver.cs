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
using SymOntoClay.Monitor.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.DataResolvers
{
    public class RelationsResolver : BaseResolver
    {
        public RelationsResolver(IMainStorageContext context)
            : base(context)
        {
            var dataResolversFactory = context.DataResolversFactory;

            _inheritanceResolver = dataResolversFactory.GetInheritanceResolver();
            _synonymsResolver = dataResolversFactory.GetSynonymsResolver();
        }

        public readonly ResolverOptions DefaultOptions = ResolverOptions.GetDefaultOptions();

        private readonly InheritanceResolver _inheritanceResolver;
        private readonly SynonymsResolver _synonymsResolver;

        public RelationDescription GetRelation(IMonitorLogger logger, StrongIdentifierValue name, int paramsCount, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return GetRelation(name, paramsCount, localCodeExecutionContext, DefaultOptions);
        }

        public RelationDescription GetRelation(IMonitorLogger logger, StrongIdentifierValue name, int paramsCount, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var storage = localCodeExecutionContext.Storage;

            var storagesList = GetStoragesList(storage, KindOfStoragesList.CodeItems);

            var optionsForInheritanceResolver = options.Clone();
            optionsForInheritanceResolver.AddSelf = true;

            var weightedInheritanceItems = _inheritanceResolver.GetWeightedInheritanceItems(localCodeExecutionContext, optionsForInheritanceResolver);

            var rawList = GetRawList(name, paramsCount, storagesList, weightedInheritanceItems);

            if (!rawList.Any())
            {
                return null;
            }

            var filteredList = Filter(rawList);

            var targetItem = ChooseTargetItem(filteredList);

            return targetItem;
        }

        private List<WeightedInheritanceResultItemWithStorageInfo<RelationDescription>> GetRawList(IMonitorLogger logger, StrongIdentifierValue name, int paramsCount, List<StorageUsingOptions> storagesList, IList<WeightedInheritanceItem> weightedInheritanceItems)
        {
            var result = NGetRawList(name, paramsCount, storagesList, weightedInheritanceItems);

            if (result.IsNullOrEmpty())
            {
                result = GetRawListFromSynonyms(name, paramsCount, storagesList, weightedInheritanceItems);
            }

            return result;
        }

        private List<WeightedInheritanceResultItemWithStorageInfo<RelationDescription>> GetRawListFromSynonyms(IMonitorLogger logger, StrongIdentifierValue name, int paramsCount, List<StorageUsingOptions> storagesList, IList<WeightedInheritanceItem> weightedInheritanceItems)
        {
            var synonymsList = _synonymsResolver.GetSynonyms(name, storagesList);

            foreach (var synonym in synonymsList)
            {
                var rawList = NGetRawList(synonym, paramsCount, storagesList, weightedInheritanceItems);

                if (rawList.IsNullOrEmpty())
                {
                    continue;
                }

                return rawList;
            }

            return new List<WeightedInheritanceResultItemWithStorageInfo<RelationDescription>>();
        }

        private List<WeightedInheritanceResultItemWithStorageInfo<RelationDescription>> NGetRawList(IMonitorLogger logger, StrongIdentifierValue name, int paramsCount, List<StorageUsingOptions> storagesList, IList<WeightedInheritanceItem> weightedInheritanceItems)
        {
            if (!storagesList.Any())
            {
                return new List<WeightedInheritanceResultItemWithStorageInfo<RelationDescription>>();
            }

            var result = new List<WeightedInheritanceResultItemWithStorageInfo<RelationDescription>>();

            foreach (var storageItem in storagesList)
            {
                var itemsList = storageItem.Storage.RelationsStorage.GetRelationsDirectly(name, paramsCount, weightedInheritanceItems);

                if (!itemsList.Any())
                {
                    continue;
                }

                var distance = storageItem.Priority;
                var storage = storageItem.Storage;

                foreach (var item in itemsList)
                {
                    result.Add(new WeightedInheritanceResultItemWithStorageInfo<RelationDescription>(item, distance, storage));
                }
            }

            return result;
        }
    }
}
