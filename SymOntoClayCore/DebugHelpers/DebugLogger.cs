using NLog;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.DebugHelpers
{
#if DEBUG
    public static class DebugLogger
    {
        public static ILogger Instance = LogManager.GetCurrentClassLogger();
    }
#endif
}
