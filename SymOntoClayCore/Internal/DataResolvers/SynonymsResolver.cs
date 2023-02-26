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
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.DataResolvers
{
    public class SynonymsResolver : BaseResolver
    {
        public SynonymsResolver(IMainStorageContext context)
            : base(context)
        {
        }

        public readonly ResolverOptions DefaultOptions = ResolverOptions.GetDefaultOptions();

        public List<StrongIdentifierValue> GetSynonyms(StrongIdentifierValue name, LocalCodeExecutionContext localCodeExecutionContext)
        {
#if DEBUG
            //Log($"name = {name}");
#endif

            var storage = localCodeExecutionContext.Storage;

            return GetSynonyms(name, storage);
        }

        public List<StrongIdentifierValue> GetSynonyms(StrongIdentifierValue name, IStorage storage)
        {
#if DEBUG
            //Log($"name = {name}");
#endif

            var storagesList = GetStoragesList(storage, KindOfStoragesList.CodeItems);

            return GetSynonyms(name, storagesList);
        }

        public List<StrongIdentifierValue> GetSynonyms(StrongIdentifierValue name, List<StorageUsingOptions> storagesList)
        {
#if DEBUG
            //Log($"name = {name}");
#endif
            
#if DEBUG
            //Log($"storagesList.Count = {storagesList.Count}");
            //foreach (var tmpStorage in storagesList)
            //{
            //    Log($"tmpStorage.Key = {tmpStorage.Key}; tmpStorage.Value.Kind = '{tmpStorage.Value.Kind}'");
            //}
#endif

            var result = new List<StrongIdentifierValue>();

            var currentProcessedList = new List<StrongIdentifierValue>() { name };

            while (currentProcessedList.Any())
            {
                var futureProcessedList = new List<StrongIdentifierValue>();

#if DEBUG
                //Log($"currentProcessedList = {currentProcessedList.WriteListToString()}");
#endif

                foreach (var processedItem in currentProcessedList)
                {
                    var synonymsList = GetSynonymsDirectly(processedItem, storagesList);

#if DEBUG
                    //Log($"synonymsList = {synonymsList.WriteListToString()}");
#endif

                    if (synonymsList == null)
                    {
                        continue;
                    }

                    foreach (var item in synonymsList)
                    {
#if DEBUG
                        //Log($"item = {item}");
#endif

                        if (item == name || result.Contains(item))
                        {
                            continue;
                        }

                        result.Add(item);
                        futureProcessedList.Add(item);
                    }
                }

#if DEBUG
                //Log($"futureProcessedList = {futureProcessedList.WriteListToString()}");
#endif

                currentProcessedList = futureProcessedList;
            }

            return result;
        }

        private List<StrongIdentifierValue> GetSynonymsDirectly(StrongIdentifierValue name, List<StorageUsingOptions> storagesList)
        {
#if DEBUG
            //Log($"name = {name}");
#endif

            if (!storagesList.Any())
            {
                return null;
            }

            var result = new List<StrongIdentifierValue>();

            foreach(var storageItem in storagesList)
            {
                var itemsList = storageItem.Storage.SynonymsStorage.GetSynonymsDirectly(name);

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
