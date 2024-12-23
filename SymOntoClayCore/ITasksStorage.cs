using SymOntoClay.Common;
using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Monitor.Common;

namespace SymOntoClay.Core
{
    public interface ITasksStorage : ISpecificStorage
    {
        #region CompoundTask
        void Append(IMonitorLogger logger, CompoundTask compoundTask);
        #endregion

        #region PrimitiveTask
        void Append(IMonitorLogger logger, PrimitiveTask primitiveTask);
        #endregion
    }
}
