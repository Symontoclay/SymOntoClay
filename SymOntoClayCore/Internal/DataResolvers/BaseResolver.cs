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

        protected List<KeyValuePair<uint, IStorage>> GetStoragesList(IStorage storage)
        {
            var result = new List<KeyValuePair<uint, IStorage>>();

            var n = 0u;

            storage.CollectChainOfStorages(result, n);

            return result;
        }

        protected List<KeyValuePair<uint, T>> Filter<T>(List<KeyValuePair<uint, T>> source) where T: IndexedAnnotatedItem
        {
            if(!source.Any())
            {
                return new List<KeyValuePair<uint, T>>();
            }

            var result = new List<KeyValuePair<uint, T>>();

            foreach(var filteredItem in source)
            {
                if(!filteredItem.Value.HasConditionalSections)
                {
                    result.Add(filteredItem);
                    continue;
                }

                throw new NotImplementedException();
            }

            return result;
        }

        protected virtual T ChooseTargetItem<T>(List<KeyValuePair<uint, T>> source) where T : IndexedAnnotatedItem
        {
            if(!source.Any())
            {
                return default;
            }

            if(source.Count == 1)
            {
                return source.Single().Value;
            }

            throw new NotImplementedException();
        }
    }
}
