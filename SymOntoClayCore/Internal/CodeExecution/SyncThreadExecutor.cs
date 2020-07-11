using SymOntoClay.Core.Internal.Threads;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeExecution
{
    public class SyncThreadExecutor: BaseThreadExecutor
    {
        public SyncThreadExecutor(IEngineContext context)
            : base(context, new SyncActivePeriodicObject())
        {
        }
    }
}
