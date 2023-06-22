using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.ConditionOfTriggerExpr;
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.Core.Internal.Helpers;
using SymOntoClay.Core.Internal.Instances.LogicConditionalTriggerObservers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Instances.LogicConditionalTriggerExecutors
{
    public class EachTriggerConditionNodeExecutor : BaseTriggerConditionNodeExecutor
    {
        public EachTriggerConditionNodeExecutor(TriggerConditionNodeObserverContext context, TriggerConditionNode condition)
            : base(context.EngineContext.Logger)
        {
            _context = context;
            _dateTimeResolver = context.EngineContext.DataResolversFactory.GetDateTimeResolver();

            _targetDuration = TriggerConditionNodeHelper.GetInt32Duration(condition);
        }

        private readonly TriggerConditionNodeObserverContext _context;

        private readonly DateTimeResolver _dateTimeResolver;

        private readonly int _targetDuration;

        /// <inheritdoc/>
        public override Value Run(List<List<Var>> varList, RuleInstance processedRuleInstance)
        {
#if DEBUG
            Log($"_targetDuration = {_targetDuration}");
            Log($"_context.SetEachSeconds = {_context.SetEachSeconds}");
#endif

            if (!_context.SetEachSeconds.HasValue)
            {
                return LogicalValue.FalseValue;
            }

            var secondsNow = _dateTimeResolver.GetCurrentSeconds();

#if DEBUG
            Log($"secondsNow = {secondsNow}");
            Log($"_context.SetEachSeconds + _targetDuration = {_context.SetEachSeconds + _targetDuration}");
#endif

            if (secondsNow > _context.SetEachSeconds + _targetDuration)
            {
#if DEBUG
                Log($"Yess !!!!!!!!");
#endif

                return LogicalValue.TrueValue;
            }

            return LogicalValue.FalseValue;
        }
    }
}
