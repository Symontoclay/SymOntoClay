using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Monitor.Common
{
    public interface IPlatformLogger
    {
        void WriteLn(string message);
        void WriteLnRawOutput(string message);
        void WriteLnRawTrace(string message);
        void WriteLnRawDebug(string message);
        void WriteLnRawInfo(string message);
        void WriteLnRawWarn(string message);
        void WriteLnRawError(string message);
        void WriteLnRawFatal(string message);
    }
}
