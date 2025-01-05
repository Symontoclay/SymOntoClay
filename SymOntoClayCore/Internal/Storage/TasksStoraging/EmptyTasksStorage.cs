using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Monitor.Common;

namespace SymOntoClay.Core.Internal.Storage.TasksStoraging
{
    public class EmptyTasksStorage : BaseEmptySpecificStorage, ITasksStorage
    {
        public EmptyTasksStorage(IStorage storage, IMonitorLogger logger)
            : base(storage, logger)
        {
        }

        /// <inheritdoc/>
        public BaseCompoundTask GetBaseCompoundTaskByName(IMonitorLogger logger, StrongIdentifierValue name, KindOfTask requestingKindOfTask)
        {
            return null;
        }

        /// <inheritdoc/>
        public BaseTask GetBaseTaskByName(IMonitorLogger logger, StrongIdentifierValue name, KindOfTask requestingKindOfTask)
        {
            return null;
        }

        #region RootTask
        /// <inheritdoc/>
        public void Append(IMonitorLogger logger, RootTask rootTask)
        {
        }

        /// <inheritdoc/>
        public RootTask GetRootTask(IMonitorLogger logger, StrongIdentifierValue name)
        {
            return null;
        }
        #endregion

        #region StrategicTask
        /// <inheritdoc/>
        public void Append(IMonitorLogger logger, StrategicTask strategicTask)
        {
        }

        /// <inheritdoc/>
        public StrategicTask GetStrategicTask(IMonitorLogger logger, StrongIdentifierValue name)
        {
            return null;
        }
        #endregion

        #region TacticalTask
        /// <inheritdoc/>
        public void Append(IMonitorLogger logger, TacticalTask tacticalTask)
        {
        }

        /// <inheritdoc/>
        public TacticalTask GetTacticalTask(IMonitorLogger logger, StrongIdentifierValue name)
        {
            return null;
        }
        #endregion

        #region CompoundTask
        /// <inheritdoc/>
        public void Append(IMonitorLogger logger, CompoundTask compoundTask)
        {
        }

        /// <inheritdoc/>
        public CompoundTask GetCompoundTaskByName(IMonitorLogger logger, StrongIdentifierValue name)
        {
            return null;
        }
        #endregion

        #region PrimitiveTask
        /// <inheritdoc/>
        public void Append(IMonitorLogger logger, PrimitiveTask primitiveTask)
        {
        }

        /// <inheritdoc/>
        public PrimitiveTask GetPrimitiveTaskByName(IMonitorLogger logger, StrongIdentifierValue name)
        {
            return null;
        }
        #endregion
    }
}
