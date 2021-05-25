using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.DataResolvers
{
    public class MethodsResolver : BaseResolver
    {
        public MethodsResolver(IMainStorageContext context)
            : base(context)
        {
        }

        private readonly ResolverOptions _defaultOptions = ResolverOptions.GetDefaultOptions();

        public NamedFunction Resolve(StrongIdentifierValue name, LocalCodeExecutionContext localCodeExecutionContext)
        {
            return Resolve(name, localCodeExecutionContext, _defaultOptions);
        }

        public NamedFunction Resolve(StrongIdentifierValue name, LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
#if DEBUG
            //Log($"name = {name}");
#endif

            var storage = localCodeExecutionContext.Storage;

            var storagesList = GetStoragesList(storage);

#if DEBUG
            //Log($"name = {name}");
            //Log($"value = {value}");
            //Log($"reason = {reason}");
            //Log($"localCodeExecutionContext = {localCodeExecutionContext}");
            //Log($"storagesList.Count = {storagesList.Count}");
            //foreach (var tmpStorage in storagesList)
            //{
            //    Log($"tmpStorage = {tmpStorage}");
            //}
#endif

            var inheritanceResolver = _context.DataResolversFactory.GetInheritanceResolver();

            var optionsForInheritanceResolver = options.Clone();
            optionsForInheritanceResolver.AddSelf = true;

            var weightedInheritanceItems = inheritanceResolver.GetWeightedInheritanceItems(localCodeExecutionContext, optionsForInheritanceResolver);

#if DEBUG
            //Log($"weightedInheritanceItems = {weightedInheritanceItems.WriteListToString()}");
#endif

            var rawList = GetRawList(name, 0, storagesList, weightedInheritanceItems);

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

            return OrderAndDistinctByInheritance(filteredList, options).FirstOrDefault().ResultItem;
        }

        public NamedFunction Resolve(StrongIdentifierValue name, Dictionary<StrongIdentifierValue, Value> namedParameters, LocalCodeExecutionContext localCodeExecutionContext)
        {
            return Resolve(name, namedParameters, localCodeExecutionContext, _defaultOptions);
        }

        public NamedFunction Resolve(StrongIdentifierValue name, Dictionary<StrongIdentifierValue, Value> namedParameters, LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
#if DEBUG
            Log($"name = {name}");
            Log($"namedParameters = {namedParameters.WriteDict_1_ToString()}");
#endif

            var storage = localCodeExecutionContext.Storage;

            var storagesList = GetStoragesList(storage);

#if DEBUG
            //Log($"name = {name}");
            //Log($"value = {value}");
            //Log($"reason = {reason}");
            //Log($"localCodeExecutionContext = {localCodeExecutionContext}");
            //Log($"storagesList.Count = {storagesList.Count}");
            //foreach (var tmpStorage in storagesList)
            //{
            //    Log($"tmpStorage = {tmpStorage}");
            //}
#endif

            var inheritanceResolver = _context.DataResolversFactory.GetInheritanceResolver();

            var optionsForInheritanceResolver = options.Clone();
            optionsForInheritanceResolver.AddSelf = true;

            var weightedInheritanceItems = inheritanceResolver.GetWeightedInheritanceItems(localCodeExecutionContext, optionsForInheritanceResolver);

#if DEBUG
            Log($"weightedInheritanceItems = {weightedInheritanceItems.WriteListToString()}");
#endif

            var rawList = GetRawList(name, namedParameters.Count, storagesList, weightedInheritanceItems);

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

            return OrderAndDistinctByInheritance(filteredList, options).FirstOrDefault().ResultItem;
        }

        public NamedFunction Resolve(StrongIdentifierValue name, List<Value> positionedParameters, LocalCodeExecutionContext localCodeExecutionContext)
        {
            return Resolve(name, positionedParameters, localCodeExecutionContext, _defaultOptions);
        }

        public NamedFunction Resolve(StrongIdentifierValue name, List<Value> positionedParameters, LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
#if DEBUG
            //Log($"name = {name}");
            //Log($"positionedParameters = {positionedParameters.WriteListToString()}");
#endif

            var storage = localCodeExecutionContext.Storage;

            var storagesList = GetStoragesList(storage);

#if DEBUG
            //Log($"name = {name}");
            //Log($"value = {value}");
            //Log($"reason = {reason}");
            //Log($"localCodeExecutionContext = {localCodeExecutionContext}");
            //Log($"storagesList.Count = {storagesList.Count}");
            //foreach (var tmpStorage in storagesList)
            //{
            //    Log($"tmpStorage = {tmpStorage}");
            //}
#endif

            var inheritanceResolver = _context.DataResolversFactory.GetInheritanceResolver();

            var optionsForInheritanceResolver = options.Clone();
            optionsForInheritanceResolver.AddSelf = true;

            var weightedInheritanceItems = inheritanceResolver.GetWeightedInheritanceItems(localCodeExecutionContext, optionsForInheritanceResolver);

#if DEBUG
            //Log($"weightedInheritanceItems = {weightedInheritanceItems.WriteListToString()}");
#endif

            var rawList = GetRawList(name, positionedParameters.Count, storagesList, weightedInheritanceItems);

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

            return OrderAndDistinctByInheritance(filteredList, options).FirstOrDefault().ResultItem;
        }

        private List<WeightedInheritanceResultItemWithStorageInfo<NamedFunction>> GetRawList(StrongIdentifierValue name, int paramsCount, List<StorageUsingOptions> storagesList, IList<WeightedInheritanceItem> weightedInheritanceItems)
        {
#if DEBUG
            //Log($"name = {name}");
            //Log($"paramsCount = {paramsCount}");
#endif

            if (!storagesList.Any())
            {
                return new List<WeightedInheritanceResultItemWithStorageInfo<NamedFunction>>();
            }

            var result = new List<WeightedInheritanceResultItemWithStorageInfo<NamedFunction>>();

            foreach (var storageItem in storagesList)
            {
                var itemsList = storageItem.Storage.MethodsStorage.GetNamedFunctionsDirectly(name, paramsCount, weightedInheritanceItems);

                if (!itemsList.Any())
                {
                    continue;
                }

                var distance = storageItem.Priority;
                var storage = storageItem.Storage;

                foreach (var item in itemsList)
                {
                    result.Add(new WeightedInheritanceResultItemWithStorageInfo<NamedFunction>(item, distance, storage));
                }
            }

            return result;
        }
    }
}
