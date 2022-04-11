using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Instances.LogicConditionalTriggerExecutors
{
    public class GroupTriggerConditionNodeExecutor : BaseTriggerConditionNodeExecutor
    {
        public GroupTriggerConditionNodeExecutor(IEntityLogger logger)
            : base(logger)
        {
        }

        public BaseTriggerConditionNodeExecutor Left { get; set; }

        /// <inheritdoc/>
        public override Value Run(List<List<Var>> varList)
        {
            return Left.Run(varList);
        }
    }
}
