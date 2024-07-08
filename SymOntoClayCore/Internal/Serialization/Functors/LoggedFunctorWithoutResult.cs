using SymOntoClay.Core.Internal.Threads;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Threading;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Serialization.Functors
{
    public class LoggedFunctorWithoutResult: BaseFunctorWithoutResult<IMonitorLogger>
    {
        public LoggedFunctorWithoutResult(IMonitorLogger logger, Action<IMonitorLogger> action, IActiveObjectContext context, ICustomThreadPool threadPool)
            : base(logger, logger, action, context, threadPool)
        {
        }
    }
}
