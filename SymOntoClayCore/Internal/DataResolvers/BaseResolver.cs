/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

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
            var result = new List<StorageUsingOptions>();

            var n = 0;

            storage.CollectChainOfStorages(result, n);

            return result;
        }

        protected List<WeightedInheritanceResultItemWithStorageInfo<T>> Filter<T>(List<WeightedInheritanceResultItemWithStorageInfo<T>> source)
            where T: IndexedAnnotatedItem
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
            where T : IndexedAnnotatedItem
        {
            if(options.JustDistinct)
            {
                return source;
            }

            return source.OrderByDescending(p => p.IsSelf).ToList();
        }

        protected virtual T ChooseTargetItem<T>(List<WeightedInheritanceResultItemWithStorageInfo<T>> source) 
            where T : IndexedAnnotatedItem
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
