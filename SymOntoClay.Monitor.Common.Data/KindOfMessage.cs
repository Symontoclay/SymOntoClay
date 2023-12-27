using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.Monitor.Common.Data
{
    public enum KindOfMessage
    {
        Unknown,
        CreateMotitorNode,
        CreateThreadLogger,
        CallMethod,
        Parameter,
        EndCallMethod,
        MethodResolving,
        EndMethodResolving,
        ActionResolving,
        EndActionResolving,
        HostMethodResolving,
        EndHostMethodResolving,
        HostMethodActivation,
        EndHostMethodActivation,
        HostMethodStarting,
        EndHostMethodStarting,
        HostMethodExecution,
        EndHostMethodExecution,
        SystemExpr,
        CodeFrame,
        LeaveThreadExecutor,
        GoBackToPrevCodeFrame,
        Cancel,
        WeakCancel,
        Output,
        Trace,
        Debug,
        Info,
        Warn,
        Error,
        Fatal
    }
}
