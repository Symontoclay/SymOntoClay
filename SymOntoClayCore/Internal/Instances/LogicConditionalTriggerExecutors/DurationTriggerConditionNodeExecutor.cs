/*MIT License

Copyright (c) 2020 - 2026 Sergiy Tolkachov

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
using SymOntoClay.Core.Internal.Instances.LogicConditionalTriggerObservers;
using System;
using System.Collections.Generic;

namespace SymOntoClay.Core.Internal.Instances.LogicConditionalTriggerExecutors
{
    public class DurationTriggerConditionNodeExecutor : BaseTriggerConditionNodeExecutor
    {
        public DurationTriggerConditionNodeExecutor(TriggerConditionNodeObserverContext context, ILocalCodeExecutionContext localCodeExecutionContext, TriggerConditionNode condition, KindOfTriggerCondition kindOfTriggerCondition)
            : base(context.EngineContext.Logger)
        {
            if (kindOfTriggerCondition == KindOfTriggerCondition.SetCondition)
            {
                throw new Exception($"Duration can not be used in {kindOfTriggerCondition}");
            }

            _context = context;
            var engineContext = context.EngineContext;
            _dateTimeProvider = engineContext.DateTimeProvider;
            _dateTimeResolver = context.EngineContext.DataResolversFactory.GetDateTimeResolver();

            _targetDuration = _dateTimeResolver.ConvertTimeValueToTicks(Logger, condition.Value, DefaultTimeValues.DurationDefaultTimeValue, localCodeExecutionContext);
        }

        private readonly TriggerConditionNodeObserverContext _context;

        private readonly DateTimeResolver _dateTimeResolver;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly ulong _targetDuration;

        /// <inheritdoc/>
        public override (Value Value, bool IsPeriodic) Run(List<List<VarInstance>> varList, RuleInstance processedRuleInstance)
        {
            if (!_context.InitialResetTime.HasValue)
            {
                return (LogicalValue.FalseValue, false);
            }

            var ticksNow = _dateTimeProvider.CurrentTicks;

            if (ticksNow > _context.InitialResetTime + _targetDuration)
            {
                return (LogicalValue.TrueValue, false);
            }

            return (LogicalValue.FalseValue, false);
        }
    }
}
