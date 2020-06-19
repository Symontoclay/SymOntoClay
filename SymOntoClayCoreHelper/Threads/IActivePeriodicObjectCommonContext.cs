using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace SymOntoClay.CoreHelper.Threads
{
    public interface IActivePeriodicObjectCommonContext
    {
        bool IsNeedWating { get; }
        AutoResetEvent AutoResetEvent { get; }
    }
}
