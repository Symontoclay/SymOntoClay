using NLog;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeExecution.Helpers
{
    public static class ActionExecutionStatusHelper
    {
        public static ProcessStatus ToProcessStatus(ActionExecutionStatus actionExecutionStatus)
        {
            switch(actionExecutionStatus)
            {
                case ActionExecutionStatus.Complete:
                    return ProcessStatus.Completed;

                case ActionExecutionStatus.Broken:
                case ActionExecutionStatus.Faulted:
                    return ProcessStatus.Faulted;

                case ActionExecutionStatus.WeakCanceled:
                    return ProcessStatus.WeakCanceled;

                case ActionExecutionStatus.Canceled:
                    return ProcessStatus.Canceled;

                default:
                    throw new ArgumentOutOfRangeException(nameof(actionExecutionStatus), actionExecutionStatus, null);
            }
        }
    }
}
