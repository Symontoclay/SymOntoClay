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
        public EachTriggerConditionNodeObserver(TriggerConditionNodeObserverContext context, TriggerConditionNode condition)
            : base(context.EngineContext.Logger)
        {
            _context = context;

            _targetDuration = TriggerConditionNodeHelper.GetInt32Duration(condition);

            var engineContext = context.EngineContext;

            _dateTimeResolver = engineContext.DataResolversFactory.GetDateTimeResolver();

            _activeObject = new AsyncActivePeriodicObject(engineContext.ActivePeriodicObjectContext);
            _activeObject.PeriodicMethod = NRun;
            _activeObject.Start();
        }

        private readonly IActivePeriodicObject _activeObject;
        private readonly DateTimeResolver _dateTimeResolver;
        private readonly TriggerConditionNodeObserverContext _context;

        private readonly int _targetDuration;

        private bool NRun(CancellationToken cancellationToken)
        {
            Thread.Sleep(100);

#if DEBUG
            //Log($"_targetDuration = {_targetDuration}");
            //Log($"_context.SetEachSeconds = {_context.SetEachSeconds}");
#endif

            if (!_context.SetEachSeconds.HasValue)
            {
                return true;
            }

            var secondsNow = _dateTimeResolver.GetCurrentSeconds();

#if DEBUG
            //Log($"secondsNow = {secondsNow}");
            //Log($"_context.SetEachSeconds + _targetDuration = {_context.SetEachSeconds + _targetDuration}");
#endif

            if (secondsNow > _context.SetEachSeconds + _targetDuration)
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
