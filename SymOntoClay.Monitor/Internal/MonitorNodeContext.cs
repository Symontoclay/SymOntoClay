using SymOntoClay.Monitor.Internal.FileCache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.Monitor.Internal
{
    public class MonitorNodeContext
    {
        public MonitorContext MonitorContext { get; set; }
        public MonitorNodeFileCache FileCache { get; set; }

        public MonitorFeatures Features { get; set; }

        public string NodeId { get; set; }
    }
}
