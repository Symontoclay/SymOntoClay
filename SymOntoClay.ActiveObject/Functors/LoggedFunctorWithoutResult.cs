using SymOntoClay.ActiveObject.Threads;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Threading;
using System;

namespace SymOntoClay.ActiveObject.Functors
{
    public class LoggedFunctorWithoutResult : BaseFunctorWithoutResult<IMonitorLogger>
    {
        public static LoggedFunctorWithoutResult Run(IMonitorLogger logger, string functorId, Action<IMonitorLogger> action, IActiveObjectContext context, ICustomThreadPool threadPool, ISerializationAnchor serializationAnchor)
        {
            var functor = new LoggedFunctorWithoutResult(logger, functorId, action, context, threadPool, serializationAnchor);
            functor.Run();
            return functor;
        }

        public LoggedFunctorWithoutResult(IMonitorLogger logger, string functorId, Action<IMonitorLogger> action, IActiveObjectContext context, ICustomThreadPool threadPool, ISerializationAnchor serializationAnchor)
            : base(logger, functorId, logger, action, context, threadPool, serializationAnchor)
        {
        }
    }

    public class LoggedFunctorWithoutResult<T> : BaseFunctorWithoutResult<IMonitorLogger, T>
    {
        public static LoggedFunctorWithoutResult<T> Run(IMonitorLogger logger, string functorId, T arg, Action<IMonitorLogger, T> action, IActiveObjectContext context, ICustomThreadPool threadPool, ISerializationAnchor serializationAnchor)
        {
            var functor = new LoggedFunctorWithoutResult<T>(logger, functorId, arg, action, context, threadPool, serializationAnchor);
            functor.Run();
            return functor;
        }

        public LoggedFunctorWithoutResult(IMonitorLogger logger, string functorId, T arg, Action<IMonitorLogger, T> action, IActiveObjectContext context, ICustomThreadPool threadPool, ISerializationAnchor serializationAnchor)
            : base(logger, functorId, logger, arg, action, context, threadPool, serializationAnchor)
        {
        }
    }

    public class LoggedFunctorWithoutResult<T1, T2> : BaseFunctorWithoutResult<IMonitorLogger, T1, T2>
    {
        public static LoggedFunctorWithoutResult<T1, T2> Run(IMonitorLogger logger, string functorId, T1 arg1, T2 arg2, Action<IMonitorLogger, T1, T2> action, IActiveObjectContext context, ICustomThreadPool threadPool, ISerializationAnchor serializationAnchor)
        {
            var functor = new LoggedFunctorWithoutResult<T1, T2>(logger, functorId, arg1, arg2, action, context, threadPool, serializationAnchor);
            functor.Run();
            return functor;
        }

        public LoggedFunctorWithoutResult(IMonitorLogger logger, string functorId, T1 arg1, T2 arg2, Action<IMonitorLogger, T1, T2> action, IActiveObjectContext context, ICustomThreadPool threadPool, ISerializationAnchor serializationAnchor)
            : base(logger, functorId, logger, arg1, arg2, action, context, threadPool, serializationAnchor)
        {
        }
    }

    public class LoggedFunctorWithoutResult<T1, T2, T3> : BaseFunctorWithoutResult<IMonitorLogger, T1, T2, T3>
    {
        public static LoggedFunctorWithoutResult<T1, T2, T3> Run(IMonitorLogger logger, string functorId, T1 arg1, T2 arg2, T3 arg3, Action<IMonitorLogger, T1, T2, T3> action, IActiveObjectContext context, ICustomThreadPool threadPool, ISerializationAnchor serializationAnchor)
        {
            var functor = new LoggedFunctorWithoutResult<T1, T2, T3>(logger, functorId, arg1, arg2, arg3, action, context, threadPool, serializationAnchor);
            functor.Run();
            return functor;
        }

        public LoggedFunctorWithoutResult(IMonitorLogger logger, string functorId, T1 arg1, T2 arg2, T3 arg3, Action<IMonitorLogger, T1, T2, T3> action, IActiveObjectContext context, ICustomThreadPool threadPool, ISerializationAnchor serializationAnchor)
            : base(logger, functorId, logger, arg1, arg2, arg3, action, context, threadPool, serializationAnchor)
        {
        }
    }

    public class LoggedFunctorWithoutResult<T1, T2, T3, T4> : BaseFunctorWithoutResult<IMonitorLogger, T1, T2, T3, T4>
    {
        public static LoggedFunctorWithoutResult<T1, T2, T3, T4> Run(IMonitorLogger logger, string functorId, T1 arg1, T2 arg2, T3 arg3, T4 arg4, Action<IMonitorLogger, T1, T2, T3, T4> action, IActiveObjectContext context, ICustomThreadPool threadPool, ISerializationAnchor serializationAnchor)
        {
            var functor = new LoggedFunctorWithoutResult<T1, T2, T3, T4>(logger, functorId, arg1, arg2, arg3, arg4, action, context, threadPool, serializationAnchor);
            functor.Run();
            return functor;
        }

        public LoggedFunctorWithoutResult(IMonitorLogger logger, string functorId, T1 arg1, T2 arg2, T3 arg3, T4 arg4, Action<IMonitorLogger, T1, T2, T3, T4> action, IActiveObjectContext context, ICustomThreadPool threadPool, ISerializationAnchor serializationAnchor)
            : base(logger, functorId, logger, arg1, arg2, arg3, arg4, action, context, threadPool, serializationAnchor)
        {
        }
    }
}
