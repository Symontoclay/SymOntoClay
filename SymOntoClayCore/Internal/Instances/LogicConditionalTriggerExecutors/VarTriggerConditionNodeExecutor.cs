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
    public class VarTriggerConditionNodeExecutor : BaseTriggerConditionNodeExecutor
    {
        public VarTriggerConditionNodeExecutor(IEngineContext engineContext, LocalCodeExecutionContext localCodeExecutionContext, TriggerConditionNode condition)
            : base(engineContext.Logger)
        {
            _varName = condition.Name;

            _localCodeExecutionContext = localCodeExecutionContext;

            _varsResolver = engineContext.DataResolversFactory.GetVarsResolver();

#if DEBUG
            //Log($"_varName = {_varName}");
#endif
        }

        private readonly StrongIdentifierValue _varName;

        private readonly LocalCodeExecutionContext _localCodeExecutionContext;
        private readonly VarsResolver _varsResolver;

        /// <inheritdoc/>
        public override Value Run(List<List<Var>> varList)
        {
            return _varsResolver.GetVarValue(_varName, _localCodeExecutionContext);
        }
    }
}
