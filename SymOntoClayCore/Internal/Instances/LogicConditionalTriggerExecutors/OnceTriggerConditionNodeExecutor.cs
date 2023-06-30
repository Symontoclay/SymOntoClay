using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.ConditionOfTriggerExpr;
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.Core.Internal.Instances.LogicConditionalTriggerObservers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Instances.LogicConditionalTriggerExecutors
{
    public class OnceTriggerConditionNodeExecutor : BaseTriggerConditionNodeExecutor
    {
        public OnceTriggerConditionNodeExecutor(TriggerConditionNodeObserverContext context, ILocalCodeExecutionContext localCodeExecutionContext, TriggerConditionNode condition, KindOfTriggerCondition kindOfTriggerCondition)
            : base(context.EngineContext.Logger)
        {
            if (kindOfTriggerCondition == KindOfTriggerCondition.ResetCondition)
            {
                throw new Exception($"Once timer can not be used in {kindOfTriggerCondition}");
            }

            _context = context;

            var engineContext = context.EngineContext;

            _dateTimeProvider = engineContext.DateTimeProvider;
            _dateTimeResolver = engineContext.DataResolversFactory.GetDateTimeResolver();

            _targetDuration = _dateTimeResolver.ConvertTimeValueToTicks(condition.Value, DefaultTimeValues.OnceTimerDefaultTimeValue, localCodeExecutionContext);
        }

        private readonly TriggerConditionNodeObserverContext _context;

        private readonly DateTimeResolver _dateTimeResolver;
        private readonly IDateTimeProvider _dateTimeProvider;

        private readonly long _targetDuration;

        private bool _wasRun;

        /// <inheritdoc/>
        public override (Value Value, bool IsPeriodic) Run(List<List<Var>> varList, RuleInstance processedRuleInstance)
        {
#if DEBUG
            //Log($"_targetDuration = {_targetDuration}");
            //Log($"_context.IsOn = {_context.IsOn}");
            //Log($"_context.InitialSetTime = {_context.InitialSetTime}");
            //Log($"_wasRun = {_wasRun}");
#endif

            if (_wasRun)
            {
                return (LogicalValue.FalseValue, false);
            }

            if (!_context.InitialSetTime.HasValue)
            {
                return (LogicalValue.FalseValue, false);
            }

            var ticksNow = _dateTimeProvider.CurrentTiks;

#if DEBUG
            //Log($"ticksNow = {ticksNow}");
            //Log($"_context.InitialSetTime + _targetDuration = {_context.InitialSetTime + _targetDuration}");
#endif

            if (ticksNow > _context.InitialSetTime + _targetDuration)
            {
#if DEBUG
                //Log($"Yess !!!!!!!!");
#endif

                _wasRun = true;

                return (LogicalValue.TrueValue, false);
            }

            return (LogicalValue.FalseValue, false);
        }
    }
}
