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

using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Ast.Expressions;
using SymOntoClay.Core.Internal.CodeModel.ConditionOfTriggerExpr;
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.Core.Internal.Instances.LogicConditionalTriggerObservers;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Instances.LogicConditionalTriggerExecutors
{
    public class BinaryOperatorTriggerConditionNodeExecutor : BaseTriggerConditionNodeExecutor
    {
        public BinaryOperatorTriggerConditionNodeExecutor(IEngineContext engineContext, ILocalCodeExecutionContext localCodeExecutionContext, TriggerConditionNode condition)
            : base(engineContext.Logger)
        {
            _kindOfOperator = condition.KindOfOperator;

            _localCodeExecutionContext = localCodeExecutionContext;

            _operatorsResolver = engineContext.DataResolversFactory.GetOperatorsResolver();

            _codeExecutor = engineContext.CodeExecutor;
        }

        private readonly KindOfOperator _kindOfOperator;
        private readonly ILocalCodeExecutionContext _localCodeExecutionContext;
        private readonly OperatorsResolver _operatorsResolver;
        private readonly ICodeExecutorComponent _codeExecutor;

        public BaseTriggerConditionNodeExecutor Left { get; set; }
        public BaseTriggerConditionNodeExecutor Right { get; set; }

        /// <inheritdoc/>
        public override Value Run(List<List<Var>> varList, RuleInstance processedRuleInstance)
        {
            var kindOfOperator = _kindOfOperator;

            if (kindOfOperator == KindOfOperator.IsNot)
            {
                kindOfOperator = KindOfOperator.Is;
            }

            var paramsList = new List<Value>();
            paramsList.Add(Left.Run(varList, processedRuleInstance));
            paramsList.Add(Right.Run(varList, processedRuleInstance));
            paramsList.Add(NullValue.Instance);

            return _codeExecutor.CallOperator(kindOfOperator, paramsList, _localCodeExecutionContext);
        }
    }
}
