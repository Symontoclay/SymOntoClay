using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.ConditionOfTriggerExpr;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Instances.LogicConditionalTriggerExecutors
{
    public class ConceptTriggerConditionNodeExecutor : BaseTriggerConditionNodeExecutor
    {
        public ConceptTriggerConditionNodeExecutor(IEntityLogger logger, TriggerConditionNode condition)
            : base(logger)
        {
            _name = condition.Name;
        }

        private readonly StrongIdentifierValue _name;

        /// <inheritdoc/>
        public override Value Run(List<List<Var>> varList)
        {
            return _name;
        }
    }
}
