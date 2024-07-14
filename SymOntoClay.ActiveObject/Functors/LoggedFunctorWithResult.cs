using SymOntoClay.ActiveObject.Threads;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Threading;
using System;

namespace SymOntoClay.ActiveObject.Functors
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

    public class LoggedFunctorWithResult<T, TResult> : BaseFunctorWithResult<IMonitorLogger, T, TResult>
    {
        public static LoggedFunctorWithResult<T, TResult> Run(IMonitorLogger logger, T arg, Func<IMonitorLogger, T, TResult> func, IActiveObjectContext context, ICustomThreadPool threadPool)
        {
            var functor = new LoggedFunctorWithResult<T, TResult>(logger, arg, func, context, threadPool);
            functor.Run();
            return functor;
        }

        public LoggedFunctorWithResult(IMonitorLogger logger, T arg, Func<IMonitorLogger, T, TResult> func, IActiveObjectContext context, ICustomThreadPool threadPool)
            : base(logger, logger, arg, func, context, threadPool)
        {
        }
    }

    public class LoggedFunctorWithResult<T1, T2, TResult> : BaseFunctorWithResult<IMonitorLogger, T1, T2, TResult>
    {
        public static LoggedFunctorWithResult<T1, T2, TResult> Run(IMonitorLogger logger, T1 arg1, T2 arg2, Func<IMonitorLogger, T1, T2, TResult> func, IActiveObjectContext context, ICustomThreadPool threadPool)
        {
            var functor = new LoggedFunctorWithResult<T1, T2, TResult>(logger, arg1, arg2, func, context, threadPool);
            functor.Run();
            return functor;
        }

        public LoggedFunctorWithResult(IMonitorLogger logger, T1 arg1, T2 arg2, Func<IMonitorLogger, T1, T2, TResult> func, IActiveObjectContext context, ICustomThreadPool threadPool)
            : base(logger, logger, arg1, arg2, func, context, threadPool)
        {
        }
    }
}
