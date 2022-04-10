using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Ast.Expressions;
using SymOntoClay.Core.Internal.CodeModel.ConditionOfTriggerExpr;
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.CoreHelper.CollectionsHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Instances.LogicConditionalTriggerExecutors
{
    public class UnaryOperatorTriggerConditionNodeExecutor : BaseTriggerConditionNodeExecutor
    {
        public UnaryOperatorTriggerConditionNodeExecutor(IEngineContext engineContext, LocalCodeExecutionContext localCodeExecutionContext, TriggerConditionNode condition)
            : base(engineContext.Logger)
        {
#if DEBUG
            Log($"condition = {condition}");
#endif

            _kindOfOperator = condition.KindOfOperator;

#if DEBUG
            Log($"_kindOfOperator = {_kindOfOperator}");
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
        public List<BaseTriggerConditionNodeExecutor> ParamsList { get; set; }

        /// <inheritdoc/>
        public override Value Run(List<List<Var>> varList)
        {
#if DEBUG
            Log($"_kindOfOperator = {_kindOfOperator}");
#endif

            if(_kindOfOperator == KindOfOperator.CallFunction)
            {
                return RunCallFunction(varList);
            }

            return RunUnaryOperator(varList);
        }

        private Value RunCallFunction(List<List<Var>> varList)
        {
            if(ParamsList.IsNullOrEmpty())
            {

            }

            throw new NotImplementedException();
        }

        private Value RunUnaryOperator(List<List<Var>> varList)
        {
            var paramsList = new List<Value>();
            paramsList.Add(Left.Run(varList));
            paramsList.Add(NullValue.Instance);

#if DEBUG
            //Log($"paramsList = {paramsList.WriteListToString()}");
#endif

            var operatorInfo = _operatorsResolver.GetOperator(_kindOfOperator, _localCodeExecutionContext);

#if DEBUG
            //Log($"operatorInfo = {operatorInfo}");
#endif

            return _codeExecutor.CallExecutableSync(operatorInfo, paramsList, _localCodeExecutionContext);
        }
    }
}
