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
        HostMethodExecution,
        EndHostMethodExecution,
        SystemExpr,
        Output,
        Trace,
        Debug,
        Info,
        Warn,
        Error,
        Fatal
    }
}
