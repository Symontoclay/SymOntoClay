using SymOntoClay.Core.Internal.CodeModel.ConditionOfTriggerExpr;
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.Core.Internal.Helpers;
using SymOntoClay.Core.Internal.Threads;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace SymOntoClay.Core.Internal.Instances.LogicConditionalTriggerObservers
{
    public class EachTriggerConditionNodeObserver : BaseTriggerConditionNodeObserver
    {
        public EachTriggerConditionNodeObserver(TriggerConditionNodeObserverContext context, ILocalCodeExecutionContext localCodeExecutionContext, TriggerConditionNode condition, KindOfTriggerCondition kindOfTriggerCondition)
            : base(context.EngineContext.Logger)
        {
            if(kindOfTriggerCondition == KindOfTriggerCondition.ResetCondition)
            {
                throw new Exception($"Each timer can not be used in {kindOfTriggerCondition}");
            }

            _context = context;

            var engineContext = context.EngineContext;

            _dateTimeProvider = engineContext.DateTimeProvider;

            _dateTimeResolver = engineContext.DataResolversFactory.GetDateTimeResolver();

            _targetDuration = _dateTimeResolver.ConvertTimeValueToTicks(condition.Value, DefaultTimeValues.EachTimerDefaultTimeValue, localCodeExecutionContext);

#if DEBUG
            //Log($"_targetDuration = {_targetDuration}");
#endif

            _activeObject = new AsyncActivePeriodicObject(engineContext.ActivePeriodicObjectContext);
            _activeObject.PeriodicMethod = NRun;
            _activeObject.Start();
        }

        private readonly IActivePeriodicObject _activeObject;
        private readonly DateTimeResolver _dateTimeResolver;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly TriggerConditionNodeObserverContext _context;

        private readonly long _targetDuration;

        private bool NRun(CancellationToken cancellationToken)
        {
            Thread.Sleep(100);

#if DEBUG
            //Log($"_targetDuration = {_targetDuration}");
            //Log($"_context.IsOn = {_context.IsOn}");
            //Log($"_context.InitialSetTime = {_context.InitialSetTime}");
#endif

            if (!_context.InitialSetTime.HasValue)
            {
                return true;
            }

            var ticksNow = _dateTimeProvider.CurrentTiks;

#if DEBUG
            //Log($"ticksNow = {ticksNow}");
            //Log($"_context.InitialSetTime + _targetDuration = {_context.InitialSetTime + _targetDuration}");
#endif

            if (ticksNow > _context.InitialSetTime + _targetDuration)
            {
#if DEBUG
                //Log($"EmitOnChanged() !!!!!!!!");
#endif

                EmitOnChanged();
            }

            return true;
        }
    }
}
