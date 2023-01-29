using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.CoreHelper.CollectionsHelpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace SymOntoClay.Core.Internal.DataResolvers
{
    public class MetadataResolver : BaseResolver
    {
        public MetadataResolver(IMainStorageContext context)
            : base(context)
        {
            _synonymsResolver = context.DataResolversFactory.GetSynonymsResolver();
        }

        private readonly SynonymsResolver _synonymsResolver;

        public CodeItem Resolve(StrongIdentifierValue prototypeName, LocalCodeExecutionContext localCodeExecutionContext)
        {
            return Resolve(prototypeName, localCodeExecutionContext, _defaultOptions);
        }

        public CodeItem Resolve(StrongIdentifierValue prototypeName, LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
#if DEBUG
            //Log($"prototypeName = {prototypeName}");
#endif

            var storage = localCodeExecutionContext.Storage;

#if DEBUG
            //var stopWatch = new Stopwatch();
            //stopWatch.Start();
#endif

            var storagesList = GetStoragesList(storage, KindOfStoragesList.CodeItems);

#if DEBUG
            //stopWatch.Stop();
            //Log($"stopWatch.Elapsed = {stopWatch.Elapsed}");
            //Log($"name = {name}");
            //Log($"localCodeExecutionContext = {localCodeExecutionContext}");
            //Log($"localCodeExecutionContext.GetHashCode() = {localCodeExecutionContext.GetHashCode()}");
            //Log($"storagesList.Count = {storagesList.Count}");
            //foreach (var tmpStorage in storagesList)
            //{
            //    //Log($"tmpStorage = {tmpStorage}");
            //    Log($"tmpStorage.Storage.Kind = {tmpStorage.Storage.Kind}");
            //}
#endif

            var rawList = GetRawMetadataList(prototypeName, storagesList);

#if DEBUG
            //Log($"rawList.Count = {rawList.Count}");
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

            if (filteredList.Count == 1)
            {
                return filteredList.Single();
            }

            throw new NotImplementedException();
        }

        private List<CodeItem> GetRawMetadataList(StrongIdentifierValue name, List<StorageUsingOptions> storagesList)
        {
#if DEBUG
            //Log($"name = {name}");
#endif

            if (!storagesList.Any())
            {
                return new List<CodeItem>();
            }

            var synonymsList = _synonymsResolver.GetSynonyms(name, storagesList);

#if DEBUG
            //Log($"synonymsList = {synonymsList.WriteListToString()}");
#endif

            var result = new List<CodeItem>();

            var itemsList = NGetRawMetadataList(name, storagesList);

#if DEBUG
            //Log($"itemsList?.Count = {itemsList?.Count}");
#endif

            if (!itemsList.IsNullOrEmpty())
            {
                result.AddRange(itemsList);
            }

            foreach (var synonym in synonymsList)
            {
                itemsList = NGetRawMetadataList(synonym, storagesList);

#if DEBUG
                //Log($"itemsList?.Count = {itemsList?.Count}");
#endif

                if (!itemsList.IsNullOrEmpty())
                {
                    result.AddRange(itemsList);
                }
            }

            return result;
        }

        private List<CodeItem> NGetRawMetadataList(StrongIdentifierValue name, List<StorageUsingOptions> storagesList)
        {
#if DEBUG
            //Log($"name = {name}");
#endif
            var result = new List<CodeItem>();

            foreach (var storageItem in storagesList)
            {
                var codeItem = storageItem.Storage.MetadataStorage.GetByName(name);

#if DEBUG
                //Log($"codeItem = {codeItem}");
#endif

                if(codeItem == null)
                {
                    continue;
                }

                result.Add(codeItem);
            }

            return result;
        }

        private readonly ResolverOptions _defaultOptions = ResolverOptions.GetDefaultOptions();
    }
}
