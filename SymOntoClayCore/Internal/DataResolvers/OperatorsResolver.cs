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

using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Ast.Expressions;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.Monitor.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.DataResolvers
{
    public class OperatorsResolver: BaseResolver
    {
        public OperatorsResolver(IMainStorageContext context)
            : base(context)
        {
            _inheritanceResolver = context.DataResolversFactory.GetInheritanceResolver();
        }

        private readonly InheritanceResolver _inheritanceResolver;

        public Operator GetOperator(IMonitorLogger logger, KindOfOperator kindOfOperator, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return GetOperator(logger, kindOfOperator, localCodeExecutionContext, _defaultOptions);
        }

        public Operator GetOperator(IMonitorLogger logger, KindOfOperator kindOfOperator, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var storage = localCodeExecutionContext.Storage;

            var storagesList = GetStoragesList(logger, storage, KindOfStoragesList.CodeItems);

            var optionsForInheritanceResolver = options.Clone();
            optionsForInheritanceResolver.AddSelf = true;

            var weightedInheritanceItems = _inheritanceResolver.GetWeightedInheritanceItems(logger, localCodeExecutionContext, optionsForInheritanceResolver);

            var rawList = GetRawList(logger, kindOfOperator, storagesList, weightedInheritanceItems);

            var filteredList = Filter(logger, rawList);

            var targetOp = ChooseTargetItem(logger, filteredList);

            return targetOp;
        }

        private List<WeightedInheritanceResultItemWithStorageInfo<Operator>> GetRawList(IMonitorLogger logger, KindOfOperator kindOfOperator, List<StorageUsingOptions> storagesList, IList<WeightedInheritanceItem> weightedInheritanceItems)
        {
            if(!storagesList.Any())
            {
                return new List<WeightedInheritanceResultItemWithStorageInfo<Operator>>();
            }

            var result = new List<WeightedInheritanceResultItemWithStorageInfo<Operator>>();

            foreach(var storageItem in storagesList)
            {
                var operatorsList = storageItem.Storage.OperatorsStorage.GetOperatorsDirectly(logger, kindOfOperator, weightedInheritanceItems);

                if(!operatorsList.Any())
                {
                    continue;
                }

                var distance = storageItem.Priority;
                var storage = storageItem.Storage;

                foreach(var op in operatorsList)
                {
                    result.Add(new WeightedInheritanceResultItemWithStorageInfo<Operator>(op, distance, storage));
                }
            }

            return result;
        }

        private readonly ResolverOptions _defaultOptions = ResolverOptions.GetDefaultOptions();
    }
}
