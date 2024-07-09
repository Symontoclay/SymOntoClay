using SymOntoClay.Core.Internal.Threads;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Threading;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Serialization.Functors
{
    public class LoggedFunctorWithResult<TResult> : BaseFunctorWithResult<IMonitorLogger, TResult>
    {
        public static LoggedFunctorWithResult<TResult> Run(IMonitorLogger logger, Func<IMonitorLogger, TResult> func, IActiveObjectContext context, ICustomThreadPool threadPool)
        {
            var functor = new LoggedFunctorWithResult<TResult>(logger, func, context, threadPool);
            functor.Run();
            return functor;
        }

        public LoggedFunctorWithResult(IMonitorLogger logger, Func<IMonitorLogger, TResult> func, IActiveObjectContext context, ICustomThreadPool threadPool)
            : base(logger, logger, func, context, threadPool)
        {
        }
    }
}
