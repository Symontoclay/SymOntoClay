using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.ConditionOfTriggerExpr;
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.Core.Internal.Helpers;
using SymOntoClay.Core.Internal.Instances.LogicConditionalTriggerObservers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.Instances.LogicConditionalTriggerExecutors
{
    public class LogicConditionalTriggerExecutor : BaseComponent
    {
        public LogicConditionalTriggerExecutor(TriggerConditionNodeObserverContext context, StrongIdentifierValue holder, TriggerConditionNode condition, BindingVariables bindingVariables)
            : base(context.EngineContext.Logger)
        {
            _context = context;
            _engineContext = context.EngineContext;
            _dateTimeProvider = _engineContext.DateTimeProvider;

            _storage = context.Storage;
            _condition = condition;
            _bindingVariables = bindingVariables;

            var dataResolversFactory = _engineContext.DataResolversFactory;

            _searcher = dataResolversFactory.GetLogicalSearchResolver();

            _localCodeExecutionContext = new LocalCodeExecutionContext();
            _localCodeExecutionContext.Storage = _storage;
            _localCodeExecutionContext.Holder = holder;
        }

        private readonly TriggerConditionNodeObserverContext _context;
        private readonly IEngineContext _engineContext;
        private readonly IStorage _storage;
        private readonly TriggerConditionNode _condition;
        private readonly BindingVariables _bindingVariables;
        private readonly LocalCodeExecutionContext _localCodeExecutionContext;
        private readonly IDateTimeProvider _dateTimeProvider;

        private readonly LogicalSearchResolver _searcher;

        public bool Run(out List<List<Var>> varList, ref List<string> foundKeys)
        {
            varList = new List<List<Var>>();

            return Run(_condition, varList, ref foundKeys);
        }

        private bool Run(TriggerConditionNode condition, List<List<Var>> varList, ref List<string> foundKeys)
        {
            var kind = condition.Kind;

            switch (kind)
            {
                case KindOfTriggerConditionNode.Fact:
                    return RunFact(condition, varList, ref foundKeys);

                case KindOfTriggerConditionNode.Duration:
                    return RunDuration(condition);

                default:
                    throw new ArgumentOutOfRangeException(nameof(kind), kind, null);
            }
        }

        private bool RunDuration(TriggerConditionNode condition)
        {
#if DEBUG
            Log($"condition = {condition}");
#endif

            if (!_context.SetSeconds.HasValue)
            {
                return false;
            }

            var targetDuration = TriggerConditionNodeHelper.GetInt32Duration(condition);

#if DEBUG
            Log($"targetDuration = {targetDuration}");
#endif

            var secondsNow = _dateTimeProvider.CurrentTiks * _dateTimeProvider.SecondsMultiplicator;

#if DEBUG
            Log($"_context.SetSeconds = {_context.SetSeconds}");
            Log($"_dateTimeProvider.CurrentTiks = {_dateTimeProvider.CurrentTiks}");
            Log($"secondsNow = {secondsNow}");
#endif

            if (secondsNow > _context.SetSeconds + targetDuration)
            {
                return true;
            }

            return false;
        }

        private bool RunFact(TriggerConditionNode condition, List<List<Var>> varList, ref List<string> foundKeys)
        {
            var searchOptions = new LogicalSearchOptions();
            searchOptions.QueryExpression = condition.RuleInstance;
            searchOptions.LocalCodeExecutionContext = _localCodeExecutionContext;

            var searchResult = _searcher.Run(searchOptions);

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

            if(!searchResult.IsSuccess)
            {
                return false;
            }

            if(searchResult.Items.Any())
            {
                var usedKeys = new List<string>();

                foreach (var foundResultItem in searchResult.Items)
                {
                    var keyForTrigger = foundResultItem.KeyForTrigger;

                    usedKeys.Add(keyForTrigger);

                    if (foundKeys.Contains(keyForTrigger))
                    {
                        continue;
                    }

                    //foundKeys.Add(keyForTrigger);

                    if (_bindingVariables.Any())
                    {
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

                foundKeys = usedKeys;
            }

            return true;
        }
    }
}
