﻿using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.DataResolvers
{
    public class CodeItemDirectivesResolver : BaseResolver
    {
        public CodeItemDirectivesResolver(IMainStorageContext context)
            : base(context)
        {
            _inheritanceResolver = context.DataResolversFactory.GetInheritanceResolver();
        }

        private readonly InheritanceResolver _inheritanceResolver;

        public List<CodeItemDirective> Resolve(LocalCodeExecutionContext localCodeExecutionContext)
        {
            return Resolve(localCodeExecutionContext, _defaultOptions);
        }

        public List<CodeItemDirective> Resolve(LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var optionsForInheritanceResolver = options.Clone();
            optionsForInheritanceResolver.AddSelf = true;

            var weightedInheritanceItems = _inheritanceResolver.GetWeightedInheritanceItems(localCodeExecutionContext, optionsForInheritanceResolver);

#if DEBUG
            //Log($"weightedInheritanceItems = {weightedInheritanceItems.WriteListToString()}");
#endif

            if(!weightedInheritanceItems.Any())
            {
                return new List<CodeItemDirective>();
            }

            var storage = localCodeExecutionContext.Storage;

#if DEBUG
            //var storagesList = GetStoragesList(storage);

            //foreach (var tmpStorage in storagesList)
            //{
            //    Log($"tmpStorage = {tmpStorage}");
            //}
#endif

            var globalStorage = GetStoragesList(storage).Single(p => p.Storage.Kind == KindOfStorage.Global).Storage;

#if DEBUG
            //Log($"globalStorage = {globalStorage}");
#endif

            var metadataStorage = globalStorage.MetadataStorage;

            var resultsDict = new Dictionary<KindOfCodeItemDirective, CodeItemDirective>();

            foreach (var weightedInheritanceItem in weightedInheritanceItems)
            {
#if DEBUG
                //Log($"weightedInheritanceItem = {weightedInheritanceItem}");
#endif

                var codeItem = metadataStorage.GetByName(weightedInheritanceItem.SuperName);

#if DEBUG
                //Log($"codeItem = {codeItem}");
#endif

                if(codeItem == null)
                {
                    continue;
                }

                var directivesList = codeItem.Directives;

                if(!directivesList.Any())
                {
                    continue;
                }

                var directivesDict = directivesList.GroupBy(p => p.KindOfCodeItemDirective).ToDictionary(p => p.Key, p => p.ToList());

                foreach(var directivesKVPItem in directivesDict)
                {
#if DEBUG
                    //Log($"directivesKVPItem.Key = {directivesKVPItem.Key}");
                    //Log($"directivesKVPItem.Value.Count = {directivesKVPItem.Value.Count}");
#endif

                    if(resultsDict.ContainsKey(directivesKVPItem.Key))
                    {
                        continue;
                    }

                    resultsDict[directivesKVPItem.Key] = directivesKVPItem.Value.Last();
                }
            }

            return resultsDict.Values.ToList();
        }

        private readonly ResolverOptions _defaultOptions = ResolverOptions.GetDefaultOptions();
    }
}
