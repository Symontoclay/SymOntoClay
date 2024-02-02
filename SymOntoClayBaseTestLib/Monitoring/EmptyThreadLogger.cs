using SymOntoClay.Monitor.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.BaseTestLib.Monitoring
{
    public class EmptyThreadLogger: EmptyLogger, IThreadLogger
    {
        public EmptyThreadLogger()
            : this(Guid.NewGuid().ToString("D"))
        { 
        }

        public EmptyThreadLogger(string threadId)
            : base(threadId)
        {
        }
    }
}
