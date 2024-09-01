using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Monitor.Common;

namespace SymOntoClay.Core
{
    public interface IOnAddingFactHandler
    {
        IAddFactOrRuleResult OnAddingFact(IMonitorLogger logger, RuleInstance fact);
    }
}
