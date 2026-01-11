/*MIT License

Copyright (c) 2020 - 2026 Sergiy Tolkachov

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

using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Monitor.Common;
using System.Collections.Generic;
using System.Linq;

namespace SymOntoClay.Core.Internal.DataResolvers
{
    public class SynonymsResolver : BaseResolver
    {
        public SynonymsResolver(IMainStorageContext context)
            : base(context)
        {
        }

        public readonly ResolverOptions DefaultOptions = ResolverOptions.GetDefaultOptions();

        public List<StrongIdentifierValue> GetSynonyms(IMonitorLogger logger, StrongIdentifierValue name, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            var storage = localCodeExecutionContext.Storage;

            return GetSynonyms(logger, name, storage);
        }

        public List<StrongIdentifierValue> GetSynonyms(IMonitorLogger logger, StrongIdentifierValue name, IStorage storage)
        {
            var storagesList = GetStoragesList(logger, storage, KindOfStoragesList.CodeItems);

            return GetSynonyms(logger, name, storagesList);
        }

        public List<StrongIdentifierValue> GetSynonyms(IMonitorLogger logger, StrongIdentifierValue name, List<StorageUsingOptions> storagesList)
        {
            var result = new List<StrongIdentifierValue>();

            var currentProcessedList = new List<StrongIdentifierValue>() { name };

            while (currentProcessedList.Any())
            {
                var futureProcessedList = new List<StrongIdentifierValue>();

                foreach (var processedItem in currentProcessedList)
                {
                    var synonymsList = GetSynonymsDirectly(logger, processedItem, storagesList);

                    if (synonymsList == null)
                    {
                        continue;
                    }

                    foreach (var item in synonymsList)
                    {
                        if (item == name || result.Contains(item))
                        {
                            continue;
                        }

                        result.Add(item);
                        futureProcessedList.Add(item);
                    }
                }

                currentProcessedList = futureProcessedList;
            }

            return result;
        }

        private List<StrongIdentifierValue> GetSynonymsDirectly(IMonitorLogger logger, StrongIdentifierValue name, List<StorageUsingOptions> storagesList)
        {
            if (!storagesList.Any())
            {
                return null;
            }

            var result = new List<StrongIdentifierValue>();

            foreach(var storageItem in storagesList)
            {
                var itemsList = storageItem.Storage.SynonymsStorage.GetSynonymsDirectly(logger, name);

                if(itemsList == null)
                {
                    continue;
                }

                result.AddRange(itemsList);
            }

            return result;
        }
    }
}
