using SymOntoClay.Monitor;
using SymOntoClay.Monitor.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.BaseTestLib.Monitoring
{
    public class TestThreadLogger : TestLogger, IThreadLogger
    {
        public TestThreadLogger(MonitorSettings monitorSettings, string threadId)
            : base(monitorSettings, threadId)
        {
        }
    }
}
