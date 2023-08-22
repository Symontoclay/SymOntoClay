using NLog;
using SymOntoClay.Monitor.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Monitor.NLog
{
    public class ThreadLoggerNLogImpementation: MonitorLoggerNLogImpementation, IThreadLogger
    {
        public ThreadLoggerNLogImpementation(Logger logger)
            : base(logger)
        {
        }
    }
}
