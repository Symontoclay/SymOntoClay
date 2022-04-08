using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.ConditionOfTriggerExpr;
using SymOntoClay.Core.Internal.Helpers;
using SymOntoClay.Core.Internal.Threads;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace SymOntoClay.Core.Internal.Instances.LogicConditionalTriggerObservers
{
    public class DurationTriggerConditionNodeObserver : BaseTriggerConditionNodeObserver
    {
        public DurationTriggerConditionNodeObserver(TriggerConditionNodeObserverContext context, TriggerConditionNode condition)
            : base(context.EngineContext.Logger)
        {
            _context = context;

            _targetDuration = TriggerConditionNodeHelper.GetInt32Duration(condition);

            var engineContext = context.EngineContext;

            _dateTimeProvider = engineContext.DateTimeProvider;

#if DEBUG
            //Log($"condition = {condition}");
            //Log($"_targetDuration = {_targetDuration}");
#endif

            _activeObject = new AsyncActivePeriodicObject(engineContext.ActivePeriodicObjectContext);
            _activeObject.PeriodicMethod = NRun;
            _activeObject.Start();
        }

        private readonly IActivePeriodicObject _activeObject;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly TriggerConditionNodeObserverContext _context;

        private readonly int _targetDuration;

        private bool NRun()
        {
            Thread.Sleep(100);

#if DEBUG
            //Log($"_dateTimeProvider.CurrentTiks = {_dateTimeProvider.CurrentTiks}");
#endif

            if (!_context.SetSeconds.HasValue)
            {
                return true;
            }

            var secondsNow = _dateTimeProvider.CurrentTiks * _dateTimeProvider.SecondsMultiplicator;

#if DEBUG
            Log($"_context.SetSeconds = {_context.SetSeconds}");
            Log($"_dateTimeProvider.CurrentTiks = {_dateTimeProvider.CurrentTiks}");
            Log($"secondsNow = {secondsNow}");
#endif

            if(secondsNow > _context.SetSeconds + _targetDuration)
            {
                EmitOnChanged();
            }

            return true;
        }

        /// <inheritdoc/>
        protected override void OnDisposed()
        {
            _activeObject.Dispose();

            base.OnDisposed();
        }
    }
}
