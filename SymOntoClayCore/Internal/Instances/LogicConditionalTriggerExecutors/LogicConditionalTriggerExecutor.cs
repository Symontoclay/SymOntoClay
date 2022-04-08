using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.ConditionOfTriggerExpr;
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.Core.Internal.Helpers;
using SymOntoClay.Core.Internal.Instances.LogicConditionalTriggerObservers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.Instances.LogicConditionalTriggerExecutors
{
    public class LogicConditionalTriggerExecutor : BaseComponent
    {        
        public LogicConditionalTriggerExecutor(TriggerConditionNodeObserverContext context, TriggerConditionNode condition, BindingVariables bindingVariables)
            : base(context.EngineContext.Logger)
        {
            _node = TriggerConditionNodeExecutorsCreator.CreateExecutors(context, bindingVariables, condition);
        }

        private readonly BaseTriggerConditionNodeExecutor _node;

        public bool Run(out List<List<Var>> varList)
        {
            return _node.Run(out varList);
        }
    }
}
