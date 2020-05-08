using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.TriggerExecution
{
    public class TriggerExecutorComponent: BaseComponent, ITriggerExecutorComponent
    {
        public TriggerExecutorComponent(IEngineContext context)
            : base(context.Logger)
        {
            _context = context;
        }

        private readonly IEngineContext _context;
    }
}
