using SymOntoClay.ActiveObject.Functors;
using SymOntoClay.ActiveObject.Threads;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Threading;
using System;

namespace TestSandbox.Threads
{
    public class TstFunctorWithResult : BaseFunctorWithResult<int>
    {
        public TstFunctorWithResult(IMonitorLogger logger, Func<int> func, IActiveObjectContext context, ICustomThreadPool threadPool)
            : base(logger, func, context, threadPool)
        {
        }
    }
}
