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
        public override Value Run(List<List<Var>> varList, RuleInstance processedRuleInstance)
        {
            if (!_context.SetSeconds.HasValue)
            {
                return LogicalValue.FalseValue;
            }

            var secondsNow = _dateTimeProvider.CurrentTiks * _dateTimeProvider.SecondsMultiplicator;

#if DEBUG
            //Log($"_context.SetSeconds = {_context.SetSeconds}");
            //Log($"_dateTimeProvider.CurrentTiks = {_dateTimeProvider.CurrentTiks}");
            //Log($"secondsNow = {secondsNow}");
#endif

            if (secondsNow > _context.SetSeconds + _targetDuration)
            {
                return LogicalValue.TrueValue;
            }

            return LogicalValue.FalseValue;
        }
    }
}
