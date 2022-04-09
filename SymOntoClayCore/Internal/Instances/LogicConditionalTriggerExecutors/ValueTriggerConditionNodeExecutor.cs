using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.ConditionOfTriggerExpr;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Instances.LogicConditionalTriggerExecutors
{
    public class ValueTriggerConditionNodeExecutor : BaseTriggerConditionNodeExecutor
    {
        public ValueTriggerConditionNodeExecutor(IEntityLogger logger, TriggerConditionNode condition)
            : base(logger)
        {
            _value = condition.Value;
        }

        private readonly Value _value;

        /// <inheritdoc/>
        public override Value Run(List<List<Var>> varList)
        {
            return _value;
        }
    }
}
