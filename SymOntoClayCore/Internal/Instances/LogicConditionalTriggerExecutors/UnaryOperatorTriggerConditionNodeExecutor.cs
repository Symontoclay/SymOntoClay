using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Ast.Expressions;
using SymOntoClay.Core.Internal.CodeModel.ConditionOfTriggerExpr;
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.CoreHelper.CollectionsHelpers;
using SymOntoClay.CoreHelper.DebugHelpers;
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
            //Log($"condition = {condition}");
#endif

            _kindOfOperator = condition.KindOfOperator;
            _isNamedParameters = condition.IsNamedParameters;

#if DEBUG
            //Log($"_kindOfOperator = {_kindOfOperator}");
            //Log($"_isNamedParameters = {_isNamedParameters}");
#endif

            if(_kindOfOperator == KindOfOperator.CallFunction)
            {
                if(condition.ParamsList.IsNullOrEmpty())
                {
                    _kindOfparameters = KindOfFunctionParameters.NoParameters;
                }
                else
                {
                    _kindOfparameters = condition.IsNamedParameters ? KindOfFunctionParameters.NamedParameters : KindOfFunctionParameters.PositionedParameters;
                }
            }

            _localCodeExecutionContext = localCodeExecutionContext;

            _operatorsResolver = engineContext.DataResolversFactory.GetOperatorsResolver();

            _codeExecutor = engineContext.CodeExecutor;
        }

        private readonly KindOfOperator _kindOfOperator;
        private readonly bool _isNamedParameters;
        private readonly KindOfFunctionParameters _kindOfparameters;
        private readonly LocalCodeExecutionContext _localCodeExecutionContext;
        private readonly OperatorsResolver _operatorsResolver;
        private readonly ICodeExecutorComponent _codeExecutor;

        public BaseTriggerConditionNodeExecutor Left { get; set; }
        public List<BaseTriggerConditionNodeExecutor> ParamsList { get; set; }

        /// <inheritdoc/>
        public override Value Run(List<List<Var>> varList)
        {
#if DEBUG
            //Log($"_kindOfOperator = {_kindOfOperator}");
#endif

            if(_kindOfOperator == KindOfOperator.CallFunction)
            {
                return RunCallFunction(varList);
            }

            return RunUnaryOperator(varList);
        }

        private Value RunCallFunction(List<List<Var>> varList)
        {
            var caller = Left.Run(varList);

#if DEBUG
            //Log($"caller = {caller}");
#endif

            var paramsList = new List<Value>();

            if(!ParamsList.IsNullOrEmpty())
            {
                foreach (var paramExecutor in ParamsList)
                {
                    paramsList.Add(paramExecutor.Run(varList));
                }
            }

#if DEBUG
            //Log($"paramsList = {paramsList.WriteListToString()}");
#endif

            return _codeExecutor.CallFunctionSync(caller, _kindOfparameters, paramsList, _localCodeExecutionContext);
        }

        private Value RunUnaryOperator(List<List<Var>> varList)
        {
            var paramsList = new List<Value>();
            paramsList.Add(Left.Run(varList));
            paramsList.Add(NullValue.Instance);

#if DEBUG
            //Log($"paramsList = {paramsList.WriteListToString()}");
#endif

            return _codeExecutor.CallOperator(_kindOfOperator, paramsList, _localCodeExecutionContext);
        }
    }
}
