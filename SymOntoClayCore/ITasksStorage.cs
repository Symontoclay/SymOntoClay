using SymOntoClay.Common;
using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Monitor.Common;
using System.Collections.Generic;

namespace SymOntoClay.Core
{
    public interface ITasksStorage : ISpecificStorage
    {
        BaseCompoundTask GetBaseCompoundTaskByName(IMonitorLogger logger, StrongIdentifierValue name, KindOfTask requestingKindOfTask);
        BaseTask GetBaseTaskByName(IMonitorLogger logger, StrongIdentifierValue name, KindOfTask requestingKindOfTask);

        #region RootTask
        void Append(IMonitorLogger logger, RootTask rootTask);
        RootTask GetRootTask(IMonitorLogger logger, StrongIdentifierValue name);
        IEnumerable<RootTask> GetAllRootTasks(IMonitorLogger logger);
        #endregion

        #region StrategicTask
        void Append(IMonitorLogger logger, StrategicTask strategicTask);
        StrategicTask GetStrategicTask(IMonitorLogger logger, StrongIdentifierValue name);
        #endregion

        #region TacticalTask
        void Append(IMonitorLogger logger, TacticalTask tacticalTask);
        TacticalTask GetTacticalTask(IMonitorLogger logger, StrongIdentifierValue name);
        #endregion

        #region CompoundTask
        void Append(IMonitorLogger logger, CompoundTask compoundTask);
        CompoundTask GetCompoundTaskByName(IMonitorLogger logger, StrongIdentifierValue name);
        #endregion

        #region PrimitiveTask
        void Append(IMonitorLogger logger, PrimitiveTask primitiveTask);
        PrimitiveTask GetPrimitiveTaskByName(IMonitorLogger logger, StrongIdentifierValue name);
        #endregion
    }
}
