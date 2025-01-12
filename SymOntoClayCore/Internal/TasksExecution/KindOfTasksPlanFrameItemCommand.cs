using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.TasksExecution
{
    public enum KindOfTasksPlanFrameItemCommand
    {
        Nop,
        ExecPrimitiveTask,
        BeginCompoundTask,
        EndCompoundTask,
        JumpTo
    }
}
