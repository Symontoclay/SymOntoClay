using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Monitor.Common;
using System.Collections.Generic;

namespace SymOntoClay.Core
{
    public interface IDirectEngine
    {
        string DirectInsertPublicFact(IMonitorLogger logger, StrongIdentifierValue factName, string text);
        string DirectInsertPublicFact(IMonitorLogger logger, RuleInstance fact);
        void DirectRemovePublicFact(IMonitorLogger logger, string id);

        string DirectInsertFact(IMonitorLogger logger, StrongIdentifierValue factName, string text);
        void DirectRemoveFact(IMonitorLogger logger, string id);

        void DirectAddVisibleStorage(IMonitorLogger logger, IStorage storage);

        void DirectAddCategories(IMonitorLogger logger, List<string> categories);
        void DirectRemoveCategories(IMonitorLogger logger, List<string> categories);

        void DirectDie();
    }
}
