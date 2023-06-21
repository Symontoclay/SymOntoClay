/*MIT License

Copyright (c) 2020 - 2023 Sergiy Tolkachov

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
using SymOntoClay.Core.Internal.DataResolvers;
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

            if (!_context.SetDurationSeconds.HasValue)
            {
                return true;
            }

            var secondsNow = _dateTimeResolver.GetCurrentSeconds();

            if (secondsNow > _context.SetDurationSeconds + _targetDuration)
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
