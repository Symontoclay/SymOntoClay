using SymOntoClay.ActiveObject.Threads;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Threading;
using System;

namespace SymOntoClay.ActiveObject.Functors
{
    public class LoggedFunctorWithResult<TResult> : BaseFunctorWithResult<IMonitorLogger, TResult>
    {
        public static LoggedFunctorWithResult<TResult> Run(IMonitorLogger logger, string functorId, Func<IMonitorLogger, TResult> func, IActiveObjectContext context, ICustomThreadPool threadPool, ISerializationAnchor serializationAnchor)
        {
            var functor = new LoggedFunctorWithResult<TResult>(logger, functorId, func, context, threadPool, serializationAnchor);
            functor.Run();
            return functor;
        }

        public LoggedFunctorWithResult(IMonitorLogger logger, string functorId, Func<IMonitorLogger, TResult> func, IActiveObjectContext context, ICustomThreadPool threadPool, ISerializationAnchor serializationAnchor)
            : base(logger, functorId, logger, func, context, threadPool, serializationAnchor)
        {
        }
    }

    public class LoggedFunctorWithResult<T, TResult> : BaseFunctorWithResult<IMonitorLogger, T, TResult>
    {
        public static LoggedFunctorWithResult<T, TResult> Run(IMonitorLogger logger, string functorId, T arg, Func<IMonitorLogger, T, TResult> func, IActiveObjectContext context, ICustomThreadPool threadPool, ISerializationAnchor serializationAnchor)
        {
            var functor = new LoggedFunctorWithResult<T, TResult>(logger, functorId, arg, func, context, threadPool, serializationAnchor);
            functor.Run();
            return functor;
        }

        public LoggedFunctorWithResult(IMonitorLogger logger, string functorId, T arg, Func<IMonitorLogger, T, TResult> func, IActiveObjectContext context, ICustomThreadPool threadPool, ISerializationAnchor serializationAnchor)
            : base(logger, functorId, logger, arg, func, context, threadPool, serializationAnchor)
        {
        }
    }

    public class LoggedFunctorWithResult<T1, T2, TResult> : BaseFunctorWithResult<IMonitorLogger, T1, T2, TResult>
    {
        public static LoggedFunctorWithResult<T1, T2, TResult> Run(IMonitorLogger logger, string functorId, T1 arg1, T2 arg2, Func<IMonitorLogger, T1, T2, TResult> func, IActiveObjectContext context, ICustomThreadPool threadPool, ISerializationAnchor serializationAnchor)
        {
            var functor = new LoggedFunctorWithResult<T1, T2, TResult>(logger, functorId, arg1, arg2, func, context, threadPool, serializationAnchor);
            functor.Run();
            return functor;
        }

        public LoggedFunctorWithResult(IMonitorLogger logger, string functorId, T1 arg1, T2 arg2, Func<IMonitorLogger, T1, T2, TResult> func, IActiveObjectContext context, ICustomThreadPool threadPool, ISerializationAnchor serializationAnchor)
            : base(logger, functorId, logger, arg1, arg2, func, context, threadPool, serializationAnchor)
        {
        }
    }
}
