using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.Core.Internal.Storage;
using SymOntoClay.Core.Internal.Storage.LogicalStorage;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.Instances
{
    public class AddingFactNonConditionalTriggerInstance: BaseAddingFactTriggerInstance
    {
        public AddingFactNonConditionalTriggerInstance(InlineTrigger trigger, BaseInstance parent, IEngineContext context, IStorage parentStorage)
            : base(trigger, parent, context, parentStorage, false)
        {
        }

        private readonly List<List<Var>> EmptyVarsList = new List<List<Var>>();

        /// <inheritdoc/>
        protected override IAddFactOrRuleResult LogicalStorage_OnAddingFact(RuleInstance ruleInstance)
        {
#if DEBUG
            //Log($"ruleInstance = {ruleInstance.ToHumanizedString()}");
#endif

            return ProcessAction(EmptyVarsList, ruleInstance);
        }
    }
}
