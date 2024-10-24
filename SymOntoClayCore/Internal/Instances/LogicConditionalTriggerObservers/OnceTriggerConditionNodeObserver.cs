/*MIT License

Copyright (c) 2020 - 2024 Sergiy Tolkachov

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

using SymOntoClay.Core.Internal.CodeModel.ConditionOfTriggerExpr;
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.Core.Internal.Threads;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace SymOntoClay.Core.Internal.Instances.LogicConditionalTriggerObservers
{
    public class OnceTriggerConditionNodeObserver : BaseTriggerConditionNodeObserver
    {
        public OnceTriggerConditionNodeObserver(TriggerConditionNodeObserverContext context, ILocalCodeExecutionContext localCodeExecutionContext, TriggerConditionNode condition, KindOfTriggerCondition kindOfTriggerCondition)
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

            _targetDuration = _dateTimeResolver.ConvertTimeValueToTicks(Logger, condition.Value, DefaultTimeValues.OnceTimerDefaultTimeValue, localCodeExecutionContext);

            _activeObject = new AsyncActivePeriodicObject(engineContext.ActivePeriodicObjectContext);
            _activeObject.PeriodicMethod = NRun;
            _activeObject.Start();
        }

        private readonly IActivePeriodicObject _activeObject;
        private readonly DateTimeResolver _dateTimeResolver;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly TriggerConditionNodeObserverContext _context;

        private readonly long _targetDuration;

        private bool _wasRun;

        private bool NRun(CancellationToken cancellationToken)
        {
            Thread.Sleep(100);

            if(_wasRun)
            {
                return false;
            }

            if (!_context.InitialSetTime.HasValue)
            {
                return true;
            }

            var ticksNow = _dateTimeProvider.CurrentTiks;

            if (ticksNow > _context.InitialSetTime + _targetDuration)
            {
                _wasRun = true;

                EmitOnChanged();
            }

            return true;
        }
    }
}
