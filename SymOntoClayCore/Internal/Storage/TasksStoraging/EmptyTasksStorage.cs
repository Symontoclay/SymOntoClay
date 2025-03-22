using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Monitor.Common;
using System.Collections.Generic;
using System.Linq;

namespace SymOntoClay.Core.Internal.Storage.TasksStoraging
{
    public class EmptyTasksStorage : BaseEmptySpecificStorage, ITasksStorage
    {
        public EmptyTasksStorage(IStorage storage, IMonitorLogger logger)
            : base(storage, logger)
        {
        }

        /// <inheritdoc/>
        public BaseCompoundHtnTask GetBaseCompoundTaskByName(IMonitorLogger logger, StrongIdentifierValue name, KindOfTask requestingKindOfTask)
        {
            return null;
        }

        /// <inheritdoc/>
        public BaseHtnTask GetBaseTaskByName(IMonitorLogger logger, StrongIdentifierValue name, KindOfTask requestingKindOfTask)
        {
            return null;
        }

        #region RootTask
        /// <inheritdoc/>
        public void Append(IMonitorLogger logger, RootHtnTask rootTask)
        {
        }

        /// <inheritdoc/>
        public RootHtnTask GetRootTask(IMonitorLogger logger, StrongIdentifierValue name)
        {
            return null;
        }

        /// <inheritdoc/>
        public IEnumerable<RootHtnTask> GetAllRootTasks(IMonitorLogger logger)
        {
            return Enumerable.Empty<RootHtnTask>();
        }
        #endregion

        #region StrategicTask
        /// <inheritdoc/>
        public void Append(IMonitorLogger logger, StrategicHtnTask strategicTask)
        {
        }

        /// <inheritdoc/>
        public StrategicHtnTask GetStrategicTask(IMonitorLogger logger, StrongIdentifierValue name)
        {
            return null;
        }
        #endregion

        #region TacticalTask
        /// <inheritdoc/>
        public void Append(IMonitorLogger logger, TacticalHtnTask tacticalTask)
        {
        }

        /// <inheritdoc/>
        public TacticalHtnTask GetTacticalTask(IMonitorLogger logger, StrongIdentifierValue name)
        {
            return null;
        }
        #endregion

        #region CompoundTask
        /// <inheritdoc/>
        public void Append(IMonitorLogger logger, CompoundHtnTask compoundTask)
        {
        }

        /// <inheritdoc/>
        public CompoundHtnTask GetCompoundTaskByName(IMonitorLogger logger, StrongIdentifierValue name)
        {
            return null;
        }
        #endregion

        #region PrimitiveTask
        /// <inheritdoc/>
        public void Append(IMonitorLogger logger, PrimitiveHtnTask primitiveTask)
        {
        }

        /// <inheritdoc/>
        public PrimitiveHtnTask GetPrimitiveTaskByName(IMonitorLogger logger, StrongIdentifierValue name)
        {
            return null;
        }
        #endregion
    }
}
