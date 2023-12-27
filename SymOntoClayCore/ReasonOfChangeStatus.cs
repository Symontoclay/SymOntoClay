using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core
{
    public enum ReasonOfChangeStatus
    {
        Unknown,
        ByConcurrentProcess,
        CouldNotBeStartedByLowPriority,
        ByParentProcess,
        ByExecutionCoordinator,
        ByTimeout
    }
}
