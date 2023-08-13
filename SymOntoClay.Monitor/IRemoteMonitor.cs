using SymOntoClay.Monitor.Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.Monitor
{
    public interface IRemoteMonitor : IDisposable
    {
        void WriteMessage(Envelope envelope);
    }
}
