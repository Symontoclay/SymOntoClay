using SymOntoClay.Core.Internal.CodeModel.ConditionOfTriggerExpr;
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
            var engineContext = context.EngineContext;

            _dateTimeProvider = engineContext.DateTimeProvider;

#if DEBUG
            Log($"condition = {condition}");
#endif
            _activeObject = new AsyncActivePeriodicObject(engineContext.ActivePeriodicObjectContext);
            _activeObject.PeriodicMethod = NRun;
            _activeObject.Start();
        }

        private readonly IActivePeriodicObject _activeObject;
        private readonly IDateTimeProvider _dateTimeProvider;

        private bool NRun()
        {
#if DEBUG
            Log($"_dateTimeProvider.CurrentTiks = {_dateTimeProvider.CurrentTiks}");
#endif

            Thread.Sleep(100);

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
