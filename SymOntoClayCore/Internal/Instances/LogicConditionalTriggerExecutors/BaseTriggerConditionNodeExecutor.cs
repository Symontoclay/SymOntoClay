using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Instances.LogicConditionalTriggerExecutors
{
    public abstract class BaseTriggerConditionNodeExecutor : BaseComponent
    {
        protected BaseTriggerConditionNodeExecutor(IEntityLogger logger)
            : base(logger)
        {
        }

        public bool Run(out List<List<Var>> varList)
        {
            varList = new List<List<Var>>();

            return OnRunRun(varList);
        }

        protected abstract bool OnRunRun(List<List<Var>> varList);

        public BaseTriggerConditionNodeExecutor Left { get; set; }
        public BaseTriggerConditionNodeExecutor Right { get; set; }
    }
}
