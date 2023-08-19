using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Monitor.Common
{
    public interface IPlatformLogger
    {
        [Obsolete("It should be replaced to Monitor or MonitorNode", true)]
        void WriteLn(string message);

        [Obsolete("It should be replaced to Monitor or MonitorNode", true)]
        void WriteLnRawLogChannel(string message);

        [Obsolete("It should be replaced to Monitor or MonitorNode", true)]
        void WriteLnRawLog(string message);

        [Obsolete("It should be replaced to Monitor or MonitorNode", true)]
        void WriteLnRawWarning(string message);

        [Obsolete("It should be replaced to Monitor or MonitorNode", true)]
        void WriteLnRawError(string message);
    }
}
