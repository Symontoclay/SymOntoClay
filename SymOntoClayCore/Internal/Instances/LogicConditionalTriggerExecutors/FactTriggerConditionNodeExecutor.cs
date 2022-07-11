/*MIT License

Copyright (c) 2020 - 2022 Sergiy Tolkachov

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
using SymOntoClay.Core.Internal.CodeModel.ConditionOfTriggerExpr;
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.Core.Internal.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.Instances.LogicConditionalTriggerExecutors
{
    public class FactTriggerConditionNodeExecutor: BaseTriggerConditionNodeExecutor
    {
        public FactTriggerConditionNodeExecutor(IEngineContext engineContext, LocalCodeExecutionContext localCodeExecutionContext, TriggerConditionNode condition, BindingVariables bindingVariables)
            : base(engineContext.Logger)
        {
            _engineContext = engineContext;

            _bindingVariables = bindingVariables;

            var dataResolversFactory = _engineContext.DataResolversFactory;

            _searcher = dataResolversFactory.GetLogicalSearchResolver();

            _searchOptions = new LogicalSearchOptions();
            _searchOptions.QueryExpression = condition.RuleInstance;
            _searchOptions.LocalCodeExecutionContext = localCodeExecutionContext;
        }

        private readonly IEngineContext _engineContext;
        private readonly BindingVariables _bindingVariables;

        private readonly LogicalSearchOptions _searchOptions;
        private readonly LogicalSearchResolver _searcher;

        private List<string> _foundKeys = new List<string>();

        /// <inheritdoc/>
        public override Value Run(List<List<Var>> varList)
        {
            var searchResult = _searcher.Run(_searchOptions);

#if DEBUG
            //Log($"searchResult = {searchResult}");
            //Log($"_condition = {DebugHelperForRuleInstance.ToString(_condition)}");
            //Log($"searchResult = {DebugHelperForLogicalSearchResult.ToString(searchResult)}");
            //foreach(var usedKey in searchResult.UsedKeysList)
            //{
            //    Log($"usedKey = {usedKey}");
            //    Log($"_context.Dictionary.GetName(usedKey) = {_context.Dictionary.GetName(usedKey)}");
            //}
#endif

#if DEBUG
            //Log($"searchResult.Items.Count = {searchResult.Items.Count}");
#endif

            if (!searchResult.IsSuccess)
            {
                _foundKeys.Clear();

                return LogicalValue.FalseValue;
            }

            if (searchResult.Items.Any())
            {
                var usedKeys = new List<string>();

                foreach (var foundResultItem in searchResult.Items)
                {
                    var keyForTrigger = foundResultItem.KeyForTrigger;

                    usedKeys.Add(keyForTrigger);

                    if (_foundKeys.Contains(keyForTrigger))
                    {
                        continue;
                    }

                    //foundKeys.Add(keyForTrigger);

                    if (_bindingVariables.Any())
                    {
                        var resultVarsList = foundResultItem.ResultOfVarOfQueryToRelationList;

#if DEBUG
                        //Log($"resultVarsList.Count = {resultVarsList.Count}");
#endif

                        if (_bindingVariables.Count != resultVarsList.Count)
                        {
                            throw new NotImplementedException();
                        }

                        var resultVarList = new List<Var>();
                        varList.Add(resultVarList);

                        foreach (var resultVar in resultVarsList)
                        {
#if DEBUG
                            //Log($"resultVar = {resultVar}");
#endif

                            var value = LogicalQueryNodeHelper.ToValue(resultVar.FoundExpression);

#if DEBUG
                            //Log($"value = {value}");
#endif

                            var destVar = _bindingVariables.GetDest(resultVar.NameOfVar);

#if DEBUG
                            //Log($"destVar = {destVar}");
#endif

                            var varItem = new Var();
                            varItem.Name = destVar;
                            varItem.Value = value;
                            varItem.TypeOfAccess = TypeOfAccess.Local;

                            resultVarList.Add(varItem);
                        }
                    }
                }

                _foundKeys = usedKeys;
            }

            return LogicalValue.TrueValue;
        }
    }
}
