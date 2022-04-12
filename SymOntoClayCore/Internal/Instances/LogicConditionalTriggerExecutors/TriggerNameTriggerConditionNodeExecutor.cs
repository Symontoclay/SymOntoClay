using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.ConditionOfTriggerExpr;
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.CoreHelper.DebugHelpers;
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
            //Log($"_triggerName = {_triggerName}");
#endif

            _localCodeExecutionContext = localCodeExecutionContext;

           _triggersResolver = engineContext.DataResolversFactory.GetTriggersResolver();
        }

        private readonly StrongIdentifierValue _triggerName;
        private readonly LocalCodeExecutionContext _localCodeExecutionContext;
        private readonly TriggersResolver _triggersResolver;

        /// <inheritdoc/>
        public override Value Run(List<List<Var>> varList)
        {
#if DEBUG
            //Log($"_triggerName = {_triggerName}");
#endif

            var namedTriggerInstance = _triggersResolver.ResolveNamedTriggerInstance(_triggerName, _localCodeExecutionContext);

#if DEBUG
            //Log($"namedTriggerInstance.NamesList = {namedTriggerInstance.NamesList.WriteListToString()}");
#endif

            if(namedTriggerInstance == null)
            {
                return NullValue.Instance;
            }

#if DEBUG
            //Log($"namedTriggerInstance.IsOn = {namedTriggerInstance.IsOn}");           
#endif

            if(namedTriggerInstance.IsOn)
            {
                return LogicalValue.TrueValue;
            }
            else
            {
                return LogicalValue.FalseValue;
            }
        }
    }
}
