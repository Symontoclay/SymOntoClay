using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.ConditionOfTriggerExpr;
using SymOntoClay.Core.Internal.Helpers;
using SymOntoClay.Core.Internal.Instances.LogicConditionalTriggerObservers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Instances.LogicConditionalTriggerExecutors
{
    public class DurationTriggerConditionNodeExecutor : BaseTriggerConditionNodeExecutor
    {
        public DurationTriggerConditionNodeExecutor(TriggerConditionNodeObserverContext context, TriggerConditionNode condition)
            : base(context.EngineContext.Logger)
        {
            _context = context;
            _dateTimeProvider = context.EngineContext.DateTimeProvider;

            _targetDuration = TriggerConditionNodeHelper.GetInt32Duration(condition);

#if DEBUG
            //Log($"_targetDuration = {_targetDuration}");
#endif
        }

        private readonly TriggerConditionNodeObserverContext _context;

        private readonly IDateTimeProvider _dateTimeProvider;

        private readonly int _targetDuration;

        /// <inheritdoc/>
        protected override bool OnRunRun(List<List<Var>> varList)
        {
            if (!_context.SetSeconds.HasValue)
            {
                return false;
            }

            var secondsNow = _dateTimeProvider.CurrentTiks * _dateTimeProvider.SecondsMultiplicator;

#if DEBUG
            //Log($"_context.SetSeconds = {_context.SetSeconds}");
            //Log($"_dateTimeProvider.CurrentTiks = {_dateTimeProvider.CurrentTiks}");
            //Log($"secondsNow = {secondsNow}");
#endif

            if (secondsNow > _context.SetSeconds + _targetDuration)
            {
                return true;
            }

            return false;
        }
    }
}
