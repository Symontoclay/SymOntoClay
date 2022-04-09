using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Instances.LogicConditionalTriggerObservers
{
    public class VarTriggerConditionNodeObserver : BaseTriggerConditionNodeObserver
    {
        public VarTriggerConditionNodeObserver(IEntityLogger logger, IStorage storage)
            : base(logger)
        {
        }
    }
}
