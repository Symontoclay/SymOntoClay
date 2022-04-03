using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.ConditionOfTriggerExpr;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Instances.LogicConditionalTriggerExecutors
{
    public class LogicConditionalTriggerExecutor : BaseComponent
    {
        public LogicConditionalTriggerExecutor(IEngineContext context, IStorage parentStorage, TriggerConditionNode condition, BindingVariables bindingVariables)
            : base(context.Logger)
        {
            _context = context;
            _parentStorage = parentStorage;
            _condition = condition;

            throw new NotImplementedException();
        }

        private readonly IEngineContext _context;
        private readonly IStorage _parentStorage;
        private readonly TriggerConditionNode _condition;

        public bool Run(out List<Var> varList)
        {
            throw new NotImplementedException();
        }
    }
}
