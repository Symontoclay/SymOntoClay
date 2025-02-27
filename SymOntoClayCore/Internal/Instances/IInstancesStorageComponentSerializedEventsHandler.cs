using SymOntoClay.Monitor.Common;
using System.Collections.Generic;

namespace SymOntoClay.Core.Internal.Instances
{
    public interface IInstancesStorageComponentSerializedEventsHandler
    {
        void NDispatchOnIdle();
        void NOnFinishProcessWithDevicesHandler(IProcessInfo sender);
        void NCancelConcurrentProcesses(IMonitorLogger logger, string callMethodId, IProcessInfo processInfo, List<IProcessInfo> concurrentProcessesInfoList);
    }
}
