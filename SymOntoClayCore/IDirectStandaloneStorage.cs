using SymOntoClay.Monitor.Common;
using System.Collections.Generic;

namespace SymOntoClay.Core
{
    public interface IDirectStandaloneStorage
    {
        void DirectAddCategories(IMonitorLogger logger, List<string> categories);
        void DirectRemoveCategories(IMonitorLogger logger, List<string> categories);
    }
}
