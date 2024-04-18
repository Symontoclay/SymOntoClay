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

using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.ConditionOfTriggerExpr;
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Instances.LogicConditionalTriggerExecutors
{
    public class TriggerNameTriggerConditionNodeExecutor : BaseTriggerConditionNodeExecutor
    {
        public TriggerNameTriggerConditionNodeExecutor(IEngineContext engineContext, ILocalCodeExecutionContext localCodeExecutionContext, TriggerConditionNode condition, KindOfTriggerCondition kindOfTriggerCondition)
            : base(engineContext.Logger)
        {
            _triggerName = condition.Name;

            _localCodeExecutionContext = localCodeExecutionContext;

           _triggersResolver = engineContext.DataResolversFactory.GetTriggersResolver();
        }

        private readonly StrongIdentifierValue _triggerName;
        private readonly ILocalCodeExecutionContext _localCodeExecutionContext;
        private readonly TriggersResolver _triggersResolver;

        /// <inheritdoc/>
        public override (Value Value, bool IsPeriodic) Run(List<List<Var>> varList, RuleInstance processedRuleInstance)
        {
            var namedTriggerInstance = _triggersResolver.ResolveNamedTriggerInstance(Logger, _triggerName, _localCodeExecutionContext);

            if(namedTriggerInstance == null)
            {
                return (NullValue.Instance, false);
            }

            if(namedTriggerInstance.IsOn)
            {
                return (LogicalValue.TrueValue, false);
            }
            else
            {
                return (LogicalValue.FalseValue, false);
            }
        }
    }
}
