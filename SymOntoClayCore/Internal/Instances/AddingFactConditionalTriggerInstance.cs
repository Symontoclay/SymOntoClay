using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.Core.Internal.Instances.LogicConditionalTriggerExecutors;
using SymOntoClay.Core.Internal.Instances.LogicConditionalTriggerObservers;
using SymOntoClay.Core.Internal.Storage;
using SymOntoClay.Core.Internal.Storage.LogicalStorage;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.Instances
{
    public class AddingFactConditionalTriggerInstance : BaseAddingFactTriggerInstance
    {
        public AddingFactConditionalTriggerInstance(InlineTrigger trigger, BaseInstance parent, IEngineContext context, IStorage parentStorage)
            : base(trigger, parent, context, parentStorage, true)
        {
            _triggerConditionNodeObserverContext = new TriggerConditionNodeObserverContext(context, _storage, parent.Name);

            _setConditionalTriggerExecutor = new LogicConditionalTriggerExecutor(_triggerConditionNodeObserverContext, trigger.SetCondition, trigger.SetBindingVariables);
        }

        private readonly TriggerConditionNodeObserverContext _triggerConditionNodeObserverContext;
        private readonly LogicConditionalTriggerExecutor _setConditionalTriggerExecutor;

        /// <inheritdoc/>
        protected override IAddFactOrRuleResult LogicalStorage_OnAddingFact(RuleInstance ruleInstance)
        {
#if DEBUG
            //Log($"ruleInstance = {ruleInstance.ToHumanizedString()}");
            //Log($"_trigger.SetCondition = {_trigger.SetCondition}");
#endif

            var isSuccsess = _setConditionalTriggerExecutor.Run(out List<List<Var>> varsList, ruleInstance);

#if DEBUG
            //Log($"isSuccsess = {isSuccsess}");
            //Log($"setVarList.Count = {varsList.Count}");
#endif

            if(!isSuccsess)
            {
                return null;
            }

            return ProcessAction(varsList, ruleInstance);
        }
    }
}
