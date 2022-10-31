/*MIT License

Copyright (c) 2020 - 2022 Sergiy Tolkachov

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.*/

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

        private bool NRun(CancellationToken cancellationToken)
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
            //Log($"_context.SetSeconds = {_context.SetSeconds}");
            //Log($"_dateTimeProvider.CurrentTiks = {_dateTimeProvider.CurrentTiks}");
            //Log($"secondsNow = {secondsNow}");
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
