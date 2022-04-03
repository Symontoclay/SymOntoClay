using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Instances.LogicConditionalTriggerObservers
{
    public abstract class BaseTriggerConditionNodeObserver : BaseComponent
    {
        protected BaseTriggerConditionNodeObserver(IEntityLogger logger)
            : base(logger)
        {
        }

        public event Action OnChanged;

        protected void EmitOnChanged()
        {
            OnChanged?.Invoke();
        }
    }
}
