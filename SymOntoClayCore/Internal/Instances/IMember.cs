using SymOntoClay.Common;
using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Monitor.Common;

namespace SymOntoClay.Core.Internal.Instances
{
    public interface IMember: IObjectToString, IObjectToShortString, IObjectToBriefString, IObjectToHumanizedString, IMonitoredHumanizedObject
    {
        KindOfMember KindOfMember { get; }
        CallResult GetValue(IMonitorLogger logger);

        bool IsHostMethodValue { get; }
        HostMethodValue AsHostMethodValue { get; }
    }
}
