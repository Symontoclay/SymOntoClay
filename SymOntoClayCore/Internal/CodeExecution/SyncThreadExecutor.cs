using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.CoreHelper.Threads;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeExecution
{
    public class SyncThreadExecutor: BaseThreadExecutor
    {
        public SyncThreadExecutor(IEntityLogger logger)
            : base(logger, new SyncActivePeriodicObject())
        {
        }

        public void Start()
        {
            _activeObject.Start();
        }
    }
}
