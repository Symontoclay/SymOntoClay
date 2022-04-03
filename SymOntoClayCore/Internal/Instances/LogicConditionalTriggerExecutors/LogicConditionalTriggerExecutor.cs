using SymOntoClay.Core.DebugHelpers;
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
    public class LogicConditionalTriggerExecutor : BaseComponent
    {
        public LogicConditionalTriggerExecutor(IEngineContext context, StrongIdentifierValue holder, IStorage storage, TriggerConditionNode condition, BindingVariables bindingVariables)
            : base(context.Logger)
        {
            _context = context;
            _storage = storage;
            _condition = condition;
            _bindingVariables = bindingVariables;

            var dataResolversFactory = context.DataResolversFactory;

            _searcher = dataResolversFactory.GetLogicalSearchResolver();

            _localCodeExecutionContext = new LocalCodeExecutionContext();
            _localCodeExecutionContext.Storage = storage;
            _localCodeExecutionContext.Holder = holder;
        }

        private readonly IEngineContext _context;
        private readonly IStorage _storage;
        private readonly TriggerConditionNode _condition;
        private readonly BindingVariables _bindingVariables;
        private readonly LocalCodeExecutionContext _localCodeExecutionContext;

        private readonly LogicalSearchResolver _searcher;

        public bool Run(out List<List<Var>> varList)
        {
            varList = new List<List<Var>>();

            return Run(_condition, varList);
        }

        private bool Run(TriggerConditionNode condition, List<List<Var>> varList)
        {
            var kind = condition.Kind;

            switch (kind)
            {
                case KindOfTriggerConditionNode.Fact:
                    return RunFact(condition, varList);

                default:
                    throw new ArgumentOutOfRangeException(nameof(kind), kind, null);
            }
        }

        private bool RunFact(TriggerConditionNode condition, List<List<Var>> varList)
        {
            var searchOptions = new LogicalSearchOptions();
            searchOptions.QueryExpression = condition.RuleInstance;
            searchOptions.LocalCodeExecutionContext = _localCodeExecutionContext;

            var searchResult = _searcher.Run(searchOptions);

#if DEBUG
            //Log($"searchResult = {searchResult}");
            //Log($"_condition = {DebugHelperForRuleInstance.ToString(_condition)}");
            Log($"searchResult = {DebugHelperForLogicalSearchResult.ToString(searchResult)}");
            //foreach(var usedKey in searchResult.UsedKeysList)
            //{
            //    Log($"usedKey = {usedKey}");
            //    Log($"_context.Dictionary.GetName(usedKey) = {_context.Dictionary.GetName(usedKey)}");
            //}
#endif

#if DEBUG
            //Log($"searchResult.Items.Count = {searchResult.Items.Count}");
#endif

            if(!searchResult.IsSuccess)
            {
                return false;
            }

            if(searchResult.Items.Any() && _bindingVariables.Any())??????
            {
                foreach (var foundResultItem in searchResult.Items)
                {
                    var keyForTrigger = foundResultItem.KeyForTrigger;

                    usedKeys.Add(keyForTrigger);

                    if (_foundKeys.Contains(keyForTrigger))
                    {
                        continue;
                    }

                    keysForAdding.Add(keyForTrigger);
                    _foundKeys.Add(keyForTrigger);

                    var resultVarsList = foundResultItem.ResultOfVarOfQueryToRelationList;

#if DEBUG
                    Log($"resultVarsList.Count = {resultVarsList.Count}");
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

            return true;
        }
    }
}
