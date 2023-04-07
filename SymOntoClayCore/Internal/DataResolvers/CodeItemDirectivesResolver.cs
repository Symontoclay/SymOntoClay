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
    public class CodeItemDirectivesResolver : BaseResolver
    {
        public CodeItemDirectivesResolver(IMainStorageContext context)
            : base(context)
        {
            var dataResolversFactory = context.DataResolversFactory;

            _inheritanceResolver = dataResolversFactory.GetInheritanceResolver();
            _metadataResolver = dataResolversFactory.GetMetadataResolver();
        }

        private readonly InheritanceResolver _inheritanceResolver;
        private readonly MetadataResolver _metadataResolver;

        public List<CodeItemDirective> Resolve(ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return Resolve(localCodeExecutionContext, _defaultOptions);
        }

        public List<CodeItemDirective> Resolve(ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var optionsForInheritanceResolver = options.Clone();
            optionsForInheritanceResolver.AddSelf = true;

            var weightedInheritanceItems = _inheritanceResolver.GetWeightedInheritanceItems(localCodeExecutionContext, optionsForInheritanceResolver);

            if(!weightedInheritanceItems.Any())
            {
                return new List<CodeItemDirective>();
            }

            var storage = localCodeExecutionContext.Storage;

#if DEBUG

#endif

            var globalStorage = GetStoragesList(storage, KindOfStoragesList.CodeItems).Single(p => p.Storage.Kind == KindOfStorage.Global).Storage;

            var resultsDict = new Dictionary<KindOfCodeItemDirective, CodeItemDirective>();

            foreach (var weightedInheritanceItem in weightedInheritanceItems)
            {
                var codeItem = _metadataResolver.Resolve(weightedInheritanceItem.SuperName, localCodeExecutionContext);

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
