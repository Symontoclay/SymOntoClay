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
        public static LoggedFunctorWithoutResult Run(IMonitorLogger logger, Action<IMonitorLogger> action, IActiveObjectContext context, ICustomThreadPool threadPool)
        {
            var functor = new LoggedFunctorWithoutResult(logger, action, context, threadPool);
            functor.Run();
            return functor;
        }

        public LoggedFunctorWithoutResult(IMonitorLogger logger, Action<IMonitorLogger> action, IActiveObjectContext context, ICustomThreadPool threadPool)
            : base(logger, logger, action, context, threadPool)
        {
        }
    }

    public class LoggedFunctorWithoutResult<T> : BaseFunctorWithoutResult<IMonitorLogger, T>
    {
        public static LoggedFunctorWithoutResult<T> Run(IMonitorLogger logger, T arg, Action<IMonitorLogger, T> action, IActiveObjectContext context, ICustomThreadPool threadPool)
        {
            var functor = new LoggedFunctorWithoutResult<T>(logger, arg, action, context, threadPool);
            functor.Run();
            return functor;
        }

        public LoggedFunctorWithoutResult(IMonitorLogger logger, T arg, Action<IMonitorLogger, T> action, IActiveObjectContext context, ICustomThreadPool threadPool)
            : base(logger, logger, arg, action, context, threadPool)
        {
        }
    }
}
