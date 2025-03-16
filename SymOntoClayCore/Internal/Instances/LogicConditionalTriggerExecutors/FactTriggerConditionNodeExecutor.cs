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

using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.ConditionOfTriggerExpr;
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.Core.Internal.Helpers;
using System.Collections.Generic;
using System.Linq;

namespace SymOntoClay.Core.Internal.Instances.LogicConditionalTriggerExecutors
{
    public class FactTriggerConditionNodeExecutor: BaseTriggerConditionNodeExecutor
    {
        public FactTriggerConditionNodeExecutor(IEngineContext engineContext, ILocalCodeExecutionContext localCodeExecutionContext, TriggerConditionNode condition, KindOfTriggerCondition kindOfTriggerCondition, BindingVariables bindingVariables)
            : base(engineContext.Logger)
        {
            _engineContext = engineContext;

            _bindingVariables = bindingVariables;

            _localCodeExecutionContext = localCodeExecutionContext;

            var dataResolversFactory = _engineContext.DataResolversFactory;

            _searcher = dataResolversFactory.GetLogicalSearchResolver();

            _searchOptions = new LogicalSearchOptions();
            _searchOptions.QueryExpression = condition.RuleInstance;
            _searchOptions.LocalCodeExecutionContext = localCodeExecutionContext;
            _searchOptions.IgnoreIfNullValueInImperativeVariables = true;
        }

        private readonly IEngineContext _engineContext;
        private readonly BindingVariables _bindingVariables;
        private readonly ILocalCodeExecutionContext _localCodeExecutionContext;

        private readonly LogicalSearchOptions _searchOptions;
        private readonly LogicalSearchResolver _searcher;

        private List<string> _foundKeys = new List<string>();

        /// <inheritdoc/>
        public override (Value Value, bool IsPeriodic) Run(List<List<VarInstance>> varList, RuleInstance processedRuleInstance)
        {
            LogicalSearchOptions targetLogicalSearchOptions = null;

            if (processedRuleInstance == null)
            {
                targetLogicalSearchOptions = _searchOptions;
            }
            else
            {
                targetLogicalSearchOptions = new LogicalSearchOptions();
                targetLogicalSearchOptions.QueryExpression = _searchOptions.QueryExpression;
                targetLogicalSearchOptions.LocalCodeExecutionContext = _localCodeExecutionContext;
                targetLogicalSearchOptions.TargetStorage = processedRuleInstance;
            }

            var searchResult = _searcher.Run(Logger, targetLogicalSearchOptions);

            if (!searchResult.IsSuccess)
            {
                _foundKeys.Clear();

                return (LogicalValue.FalseValue, false);
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


                    if (_bindingVariables.Any())
                    {
                        var resultVarsList = foundResultItem.ResultOfVarOfQueryToRelationList;

                        var resultVarList = new List<VarInstance>();
                        varList.Add(resultVarList);

                        if (processedRuleInstance != null)
                        {
                            var destVar = _bindingVariables.GetDest(StrongIdentifierValue.LogicalVarBlankIdentifier);

                            if(destVar != null)
                            {
                                processedRuleInstance.CheckDirty();

                                var varItem = new VarInstance(destVar, TypeOfAccess.Local, _engineContext);
                                
                                varItem.SetValueDirectly(Logger, processedRuleInstance);                                

                                resultVarList.Add(varItem);
                            }                            
                        }

                        foreach (var resultVar in resultVarsList)
                        {
                            var value = LogicalQueryNodeHelper.ToValue(resultVar.FoundExpression);

                            var destVar = _bindingVariables.GetDest(resultVar.NameOfVar);

                            if(destVar == null)
                            {
                                continue;
                            }

                            var varItem = new VarInstance(destVar, TypeOfAccess.Local, _engineContext);
                            varItem.SetValueDirectly(Logger, value);
                            
                            resultVarList.Add(varItem);
                        }
                    }
                }

                _foundKeys = usedKeys;
            }

            return (LogicalValue.TrueValue, false);
        }
    }
}
