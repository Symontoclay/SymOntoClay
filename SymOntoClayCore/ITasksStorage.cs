using SymOntoClay.Common;
using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Monitor.Common;
using System.Collections.Generic;

namespace SymOntoClay.Core
{
    public interface ITasksStorage : ISpecificStorage
    {
        BaseCompoundTask GetBaseCompoundTaskByName(IMonitorLogger logger, StrongIdentifierValue name);
        BaseTask GetBaseTaskByName(IMonitorLogger logger, StrongIdentifierValue name);

        #region RootTask
        void Append(IMonitorLogger logger, RootTask rootTask);
        RootTask
        #endregion

        #region StrategicTask
        void Append(IMonitorLogger logger, StrategicTask strategicTask);
        StrategicTask
        #endregion

        #region TacticalTask
        void Append(IMonitorLogger logger, TacticalTask tacticalTask);
        TacticalTask
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
