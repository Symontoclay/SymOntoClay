using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Monitor.Common
{
    public interface IMonitoredHumanizedObject
    {
        string ToHumanizedString(IMonitorLogger logger);
    }
}
