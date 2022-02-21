/*MIT License

Copyright (c) 2020 - <curr_year/> Sergiy Tolkachov

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
using SymOntoClay.Core.Internal.IndexedData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.DataResolvers
{
    public abstract class BaseResolver: BaseLoggedComponent
    {
        protected BaseResolver(IMainStorageContext context)
            : base(context.Logger)
        {
            _context = context;
        }

        protected readonly IMainStorageContext _context;

        protected List<StorageUsingOptions> GetStoragesList(IStorage storage)
        {
            return GetStoragesList(storage, null);
        }

        protected List<StorageUsingOptions> GetStoragesList(IStorage storage, CollectChainOfStoragesOptions options)
        {
            var result = new List<StorageUsingOptions>();

            var n = 0;

            if(options != null && options.InitialPriority.HasValue)
            {
                n = options.InitialPriority.Value;
            }

            var usedStorages = new List<IStorage>();

            storage.CollectChainOfStorages(result, usedStorages, n, options);

            return result;
        }

        protected List<WeightedInheritanceResultItemWithStorageInfo<T>> Filter<T>(List<WeightedInheritanceResultItemWithStorageInfo<T>> source)
            where T: AnnotatedItem
        {
            if(!source.Any())
            {
                return new List<WeightedInheritanceResultItemWithStorageInfo<T>>();
            }

            var result = new List<WeightedInheritanceResultItemWithStorageInfo<T>>();

            foreach(var filteredItem in source)
            {
                if(!filteredItem.ResultItem.HasConditionalSections)
                {
                    result.Add(filteredItem);
                    continue;
                }

                throw new NotImplementedException();
            }

            return result;
        }

        protected List<WeightedInheritanceResultItemWithStorageInfo<T>> OrderAndDistinctByInheritance<T>(List<WeightedInheritanceResultItemWithStorageInfo<T>> source, ResolverOptions options)
            where T : AnnotatedItem
        {
            if(options.JustDistinct)
            {
                return source;
            }

            return source.OrderByDescending(p => p.IsSelf).ThenByDescending(p => p.Rank).ThenBy(p => p.Distance).ToList();
        }

        protected virtual T ChooseTargetItem<T>(List<WeightedInheritanceResultItemWithStorageInfo<T>> source) 
            where T : AnnotatedItem
        {
            if(!source.Any())
            {
                return default;
            }

            if(source.Count == 1)
            {
                return source.Single().ResultItem;
            }

            throw new NotImplementedException();
        }
    }
}
