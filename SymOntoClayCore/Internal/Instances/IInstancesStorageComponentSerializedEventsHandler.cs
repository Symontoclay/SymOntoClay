using SymOntoClay.Monitor.Common;
using System.Collections.Generic;

namespace SymOntoClay.Core.Internal.Instances
{
    public interface IInstancesStorageComponentSerializedEventsHandler
    {
        void NDelayedInitialDispatchIdleActions(IMonitorLogger logger);
        void NDispatchOnIdle();
        void NOnFinishProcessWithDevicesHandler(IProcessInfo sender);
        void NOnFinishProcessWithoutDevicesHandler(IProcessInfo sender);
        void NCancelConcurrentProcesses(IMonitorLogger logger, string callMethodId, IProcessInfo processInfo, List<IProcessInfo> concurrentProcessesInfoList);
    }
}
