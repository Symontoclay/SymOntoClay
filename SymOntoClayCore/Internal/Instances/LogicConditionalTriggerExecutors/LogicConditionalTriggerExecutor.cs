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

using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.ConditionOfTriggerExpr;
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.Core.Internal.Helpers;
using SymOntoClay.Core.Internal.Instances.LogicConditionalTriggerObservers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.Instances.LogicConditionalTriggerExecutors
{
    public class LogicConditionalTriggerExecutor : BaseComponent
    {        
        public LogicConditionalTriggerExecutor(TriggerConditionNodeObserverContext context, TriggerConditionNode condition, KindOfTriggerCondition kindOfTriggerCondition, BindingVariables bindingVariables, ILocalCodeExecutionContext parentCodeExecutionContext)
            : base(context.EngineContext.Logger)
        {
            _toSystemBoolResolver = context.EngineContext.DataResolversFactory.GetToSystemBoolResolver();

            var localCodeExecutionContext = new LocalCodeExecutionContext(parentCodeExecutionContext);
            localCodeExecutionContext.Storage = context.Storage;
            localCodeExecutionContext.Holder = context.Holder;

            _localCodeExecutionContext = localCodeExecutionContext;

            _node = TriggerConditionNodeExecutorsCreator.CreateExecutors(context, localCodeExecutionContext, bindingVariables, condition, kindOfTriggerCondition);
        }

        private ToSystemBoolResolver _toSystemBoolResolver;
        private readonly BaseTriggerConditionNodeExecutor _node;
        private readonly ILocalCodeExecutionContext _localCodeExecutionContext;

        public ILocalCodeExecutionContext LocalCodeExecutionContext => _localCodeExecutionContext;

        public (bool IsSuccess, bool IsPeriodic) Run(out List<List<Var>> varList)
        {
            return Run(out varList, null);
        }

        public (bool IsSuccess, bool IsPeriodic) Run(out List<List<Var>> varList, RuleInstance processedRuleInstance)
        {
            varList = new List<List<Var>>();

            var initialResult = _node.Run(varList, processedRuleInstance);

            return (_toSystemBoolResolver.Resolve(initialResult.Value), initialResult.IsPeriodic);
        }
    }
}
