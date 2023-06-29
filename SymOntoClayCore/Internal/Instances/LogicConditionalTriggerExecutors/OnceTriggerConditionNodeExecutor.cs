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

        /// <inheritdoc/>
        public override (Value Value, bool IsPeriodic) Run(List<List<Var>> varList, RuleInstance processedRuleInstance)
        {
#if DEBUG
            Log($"_targetDuration = {_targetDuration}");
            //Log($"_context.IsOn = {_context.IsOn}");
            Log($"_context.InitialEachTime = {_context.InitialEachTime}");
#endif

            throw new NotImplementedException();
        }
    }
}
