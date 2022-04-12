using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.ConditionOfTriggerExpr;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Instances.LogicConditionalTriggerExecutors
{
    public class TriggerNameTriggerConditionNodeExecutor : BaseTriggerConditionNodeExecutor
    {
        public TriggerNameTriggerConditionNodeExecutor(IEngineContext engineContext, LocalCodeExecutionContext localCodeExecutionContext, TriggerConditionNode condition)
            : base(engineContext.Logger)
        {
            _triggerName = condition.Name;

#if DEBUG
            Log($"_triggerName = {_triggerName}");
#endif
        }

        private readonly StrongIdentifierValue _triggerName;

        /// <inheritdoc/>
        public override Value Run(List<List<Var>> varList)
        {
            throw new NotImplementedException();
        }
    }
}
