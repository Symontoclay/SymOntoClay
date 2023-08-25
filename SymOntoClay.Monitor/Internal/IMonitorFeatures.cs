using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Monitor.Internal
{
    public interface IMonitorFeatures : IObjectToString
    {
        bool EnableCallMethod { get; }
        bool EnableParameter { get; }
        bool EnableOutput { get; }
        bool EnableTrace { get; }
        bool EnableDebug { get; }
        bool EnableInfo { get; }
        bool EnableWarn { get; }
        bool EnableError { get; }
        bool EnableFatal { get; }
    }
}
