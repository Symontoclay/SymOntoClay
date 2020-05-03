using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.CoreHelper.DebugHelpers
{
    public interface ILogger
    {
        void Log(string message);
        void Error(string message);
        void Warning(string message);
    }
}
