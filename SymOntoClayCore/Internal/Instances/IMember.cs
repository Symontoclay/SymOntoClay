using SymOntoClay.Common;
using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Monitor.Common;

namespace SymOntoClay.Core.Internal.Instances
{
    public interface IMember: IObjectToString, IObjectToShortString, IObjectToBriefString, IObjectToHumanizedString, IMonitoredHumanizedObject
    {
    }
}
