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

using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Ast.Expressions;
using SymOntoClay.Core.Internal.CodeModel.ConditionOfTriggerExpr;
using SymOntoClay.Core.Internal.DataResolvers;
using System.Collections.Generic;

namespace SymOntoClay.Core.Internal.Instances.LogicConditionalTriggerExecutors
{
    public class BinaryOperatorTriggerConditionNodeExecutor : BaseTriggerConditionNodeExecutor
    {
        public BinaryOperatorTriggerConditionNodeExecutor(IEngineContext engineContext, ILocalCodeExecutionContext localCodeExecutionContext, TriggerConditionNode condition, KindOfTriggerCondition kindOfTriggerCondition)
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
        public override (Value Value, bool IsPeriodic) Run(List<List<VarInstance>> varList, RuleInstance processedRuleInstance)
        {
            var kindOfOperator = _kindOfOperator;

            if (kindOfOperator == KindOfOperator.IsNot)
            {
                kindOfOperator = KindOfOperator.Is;
            }

            var leftResult = Left.Run(varList, processedRuleInstance);
            var rightResult = Right.Run(varList, processedRuleInstance);

            var paramsList = new List<Value>();
            paramsList.Add(leftResult.Value);
            paramsList.Add(rightResult.Value);
            paramsList.Add(NullValue.Instance);

            return (_codeExecutor.CallOperator(Logger, kindOfOperator, paramsList, _localCodeExecutionContext, CallMode.Default), leftResult.IsPeriodic || rightResult.IsPeriodic);
        }
    }
}
