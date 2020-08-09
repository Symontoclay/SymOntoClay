using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core
{
    public enum ProcessStatus
    {
        Created,
        WaitingToRun,
        Running,
        WaitingForChildren,
        WaitingForResources,
        WaitingForChildrenToComplete,
        Completed,
        Canceled,
        Faulted
    }
}
