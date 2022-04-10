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
        public BinaryOperatorTriggerConditionNodeExecutor(IEngineContext engineContext, LocalCodeExecutionContext localCodeExecutionContext, TriggerConditionNode condition)
            : base(engineContext.Logger)
        {
#if DEBUG
            //Log($"condition = {condition}");
#endif

            _kindOfOperator = condition.KindOfOperator;

#if DEBUG
            //Log($"_kindOfOperator = {_kindOfOperator}");
#endif

            _localCodeExecutionContext = localCodeExecutionContext;

            _operatorsResolver = engineContext.DataResolversFactory.GetOperatorsResolver();

            _codeExecutor = engineContext.CodeExecutor;
        }

        private readonly KindOfOperator _kindOfOperator;
        private readonly LocalCodeExecutionContext _localCodeExecutionContext;
        private readonly OperatorsResolver _operatorsResolver;
        private readonly ICodeExecutorComponent _codeExecutor;

        public BaseTriggerConditionNodeExecutor Left { get; set; }
        public BaseTriggerConditionNodeExecutor Right { get; set; }

        /// <inheritdoc/>
        public override Value Run(List<List<Var>> varList)
        {
            var kindOfOperator = _kindOfOperator;

            if (kindOfOperator == KindOfOperator.IsNot)
            {
                kindOfOperator = KindOfOperator.Is;
            }

#if DEBUG
            //Log($"kindOfOperator = {kindOfOperator}");
#endif

            var paramsList = new List<Value>();
            paramsList.Add(Left.Run(varList));
            paramsList.Add(Right.Run(varList));
            paramsList.Add(NullValue.Instance);

#if DEBUG
            //Log($"paramsList = {paramsList.WriteListToString()}");
#endif

            return _codeExecutor.CallOperator(kindOfOperator, paramsList, _localCodeExecutionContext);
        }
    }
}
