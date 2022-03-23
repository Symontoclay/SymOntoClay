using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Instances
{
    public class StateDeactivator : BaseTriggerInstance
    {
        public StateDeactivator(RuleInstance condition, BaseInstance parent, IEngineContext context, IStorage parentStorage)
            : base(condition, parent, context, parentStorage)
        {
        }

        /// <inheritdoc/>
        protected override void RunHandler(LocalCodeExecutionContext localCodeExecutionContext)
        {
#if DEBUG
            Log("Begin");
#endif
        }
    }
}
