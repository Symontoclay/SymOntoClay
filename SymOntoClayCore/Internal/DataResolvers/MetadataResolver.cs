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
using SymOntoClay.Monitor.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SymOntoClay.Core.Internal.DataResolvers
{
    public class MetadataResolver : BaseResolver
    {
        public MetadataResolver(IMainStorageContext context)
            : base(context)
        {
            
        }

        /// <inheritdoc/>
        protected override void LinkWithOtherBaseContextComponents()
        {
            base.LinkWithOtherBaseContextComponents();

            _synonymsResolver = _context.DataResolversFactory.GetSynonymsResolver();
        }

        private SynonymsResolver _synonymsResolver;

        public CodeItem Resolve(IMonitorLogger logger, StrongIdentifierValue prototypeName, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return Resolve(logger, prototypeName, localCodeExecutionContext, _defaultOptions);
        }

        public CodeItem Resolve(IMonitorLogger logger, StrongIdentifierValue prototypeName, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var storage = localCodeExecutionContext.Storage;

            var storagesList = GetStoragesList(logger, storage, KindOfStoragesList.CodeItems);

            var rawList = GetRawMetadataList(logger, prototypeName, storagesList);

            if (!rawList.Any())
            {
                return null;
            }

            var filteredList = Filter(logger, rawList);

            if (!filteredList.Any())
            {
                return null;
            }

            if (filteredList.Count == 1)
            {
                return filteredList.Single();
            }

            throw new NotImplementedException("243B4D7F-BF65-49F3-9F38-27B72504C848");
        }

        private List<CodeItem> GetRawMetadataList(IMonitorLogger logger, StrongIdentifierValue name, List<StorageUsingOptions> storagesList)
        {
            if (!storagesList.Any())
            {
                return new List<CodeItem>();
            }

            var synonymsList = _synonymsResolver.GetSynonyms(logger, name, storagesList);

            var result = new List<CodeItem>();

            var itemsList = NGetRawMetadataList(logger, name, storagesList);

            if (!itemsList.IsNullOrEmpty())
            {
                result.AddRange(itemsList);
            }

            foreach (var synonym in synonymsList)
            {
                itemsList = NGetRawMetadataList(logger, synonym, storagesList);

                if (!itemsList.IsNullOrEmpty())
                {
                    result.AddRange(itemsList);
                }
            }

            return result;
        }

        private List<CodeItem> NGetRawMetadataList(IMonitorLogger logger, StrongIdentifierValue name, List<StorageUsingOptions> storagesList)
        {
            var result = new List<CodeItem>();

            foreach (var storageItem in storagesList)
            {
                var codeItem = storageItem.Storage.MetadataStorage.GetByName(logger, name);

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
