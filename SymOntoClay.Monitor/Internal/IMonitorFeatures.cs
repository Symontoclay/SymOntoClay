using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Monitor.Internal
{
    public interface IMonitorFeatures : IObjectToString
    {
        bool EnableCallMethod { get; }
        bool EnableParameter { get; }
        bool EnableEndCallMethod { get; }
        bool EnableMethodResolving { get; }
        bool EnableEndMethodResolving { get; }
        bool EnableActionResolving { get; }
        bool EnableEndActionResolving { get; }
        bool EnableHostMethodResolving { get; }
        bool EnableEndHostMethodResolving { get; }
        bool EnableHostMethodActivation { get; }
        bool EnableEndHostMethodActivation { get; }
        bool EnableHostMethodStarting { get; }
        bool EnableEndHostMethodStarting { get; }
        bool EnableHostMethodExecution { get; }
        bool EnableEndHostMethodExecution { get; }
        bool EnableSystemExpr { get; }
        bool EnableCodeFrame { get; }
        bool EnableLeaveThreadExecutor { get; }
        bool EnableGoBackToPrevCodeFrame { get; }
        bool EnableCancelProcessInfo { get; }
        bool EnableWeakCancelProcessInfo { get; }
        bool EnableCancelInstanceExecution { get; }
        bool EnableSetExecutionCoordinatorStatus { get; }
        bool EnableSetProcessInfoStatus { get; }
        bool EnableOutput { get; }
        bool EnableTrace { get; }
        bool EnableDebug { get; }
        bool EnableInfo { get; }
        bool EnableWarn { get; }
        bool EnableError { get; }
        bool EnableFatal { get; }
    }
}
