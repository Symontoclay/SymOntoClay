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
using SymOntoClay.Core.Internal.Instances.LogicConditionalTriggerObservers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Instances.LogicConditionalTriggerExecutors
{
    public class EachTriggerConditionNodeExecutor : BaseTriggerConditionNodeExecutor
    {
        public EachTriggerConditionNodeExecutor(TriggerConditionNodeObserverContext context, ILocalCodeExecutionContext localCodeExecutionContext, TriggerConditionNode condition, KindOfTriggerCondition kindOfTriggerCondition)
            : base(context.EngineContext.Logger)
        {
            if (kindOfTriggerCondition == KindOfTriggerCondition.ResetCondition)
            {
                throw new Exception($"Each timer can not be used in {kindOfTriggerCondition}");
            }

            _context = context;

            var engineContext = context.EngineContext;

            _dateTimeProvider = engineContext.DateTimeProvider;
            _dateTimeResolver = engineContext.DataResolversFactory.GetDateTimeResolver();

            _targetDuration = _dateTimeResolver.ConvertTimeValueToTicks(condition.Value, DefaultTimeValues.EachTimerDefaultTimeValue, localCodeExecutionContext);
        }

        private readonly TriggerConditionNodeObserverContext _context;

        private readonly DateTimeResolver _dateTimeResolver;
        private readonly IDateTimeProvider _dateTimeProvider;

        private readonly long _targetDuration;

        /// <inheritdoc/>
        public override (Value Value, bool IsPeriodic) Run(List<List<Var>> varList, RuleInstance processedRuleInstance)
        {
#if DEBUG
            //Log($"_targetDuration = {_targetDuration}");
            //Log($"_context.IsOn = {_context.IsOn}");
            //Log($"_context.InitialSetTime = {_context.InitialSetTime}");
#endif

            if (!_context.InitialSetTime.HasValue)
            {
                return (LogicalValue.FalseValue, true);
            }

            var ticksNow = _dateTimeProvider.CurrentTiks;

#if DEBUG
            //Log($"ticksNow = {ticksNow}");
            //Log($"_context.InitialSetTime + _targetDuration = {_context.InitialSetTime + _targetDuration}");
#endif

            if (ticksNow > _context.InitialSetTime + _targetDuration)
            {
#if DEBUG
                //Log($"Yess !!!!!!!!");
#endif

                return (LogicalValue.TrueValue, true);
            }

            return (LogicalValue.FalseValue, true);
        }
    }
}
