using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Monitor.Common;

namespace SymOntoClay.Core.Internal.Storage.LogicalStoraging
{
    public interface IConsolidatedPublicFactsLogicalStorageSerializedEventsHandler
    {
        void NIsolatedProcessNewFact(IMonitorLogger logger, RuleInstance ruleInstance);
    }
}
