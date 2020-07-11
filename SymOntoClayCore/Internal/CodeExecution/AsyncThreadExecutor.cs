using SymOntoClay.Core.Internal.Threads;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeExecution
{
    public class AsyncThreadExecutor: BaseThreadExecutor
    {
        public AsyncThreadExecutor(IEngineContext context)
            : base(context, new AsyncActivePeriodicObject(context.ActivePeriodicObjectContext))
        {
        }
    }
}
