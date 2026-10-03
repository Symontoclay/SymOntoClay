using SymOntoClay.Common.Cancellation;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Threading;
using System;

namespace SymOntoClay.ActiveObject.Functors
{
    public class LoggedAltFunctorWithoutResult : BaseAltFunctorWithoutResult<IMonitorLogger>
    {
        public static LoggedAltFunctorWithoutResult Run(IMonitorLogger logger, string functorId, Action<IMonitorLogger> action, ICustomThreadPool threadPool, ICancellationContext cancellationContext, ISerializationAnchor serializationAnchor)
        {
            var functor = new LoggedAltFunctorWithoutResult(logger, functorId, action, threadPool, cancellationContext, serializationAnchor);
            functor.Run();
            return functor;
        }

        public LoggedAltFunctorWithoutResult(IMonitorLogger logger, string functorId, Action<IMonitorLogger> action, ICustomThreadPool threadPool, ICancellationContext cancellationContext, ISerializationAnchor serializationAnchor)
            : base(logger, functorId, logger, action, threadPool, cancellationContext, serializationAnchor)
        {
        }
    }

    public class LoggedAltFunctorWithoutResult<T> : BaseAltFunctorWithoutResult<IMonitorLogger, T>
    {
        public static LoggedAltFunctorWithoutResult<T> Run(IMonitorLogger logger, string functorId, T arg, Action<IMonitorLogger, T> action, ICustomThreadPool threadPool, ICancellationContext cancellationContext, ISerializationAnchor serializationAnchor)
        {
            var functor = new LoggedAltFunctorWithoutResult<T>(logger, functorId, arg, action, threadPool, cancellationContext, serializationAnchor);
            functor.Run();
            return functor;
        }

        public LoggedAltFunctorWithoutResult(IMonitorLogger logger, string functorId, T arg, Action<IMonitorLogger, T> action, ICustomThreadPool threadPool, ICancellationContext cancellationContext, ISerializationAnchor serializationAnchor)
            : base(logger, functorId, logger, arg, action, threadPool, cancellationContext, serializationAnchor)
        {
        }
    }

    public class LoggedAltFunctorWithoutResult<T1, T2> : BaseAltFunctorWithoutResult<IMonitorLogger, T1, T2>
    {
        public static LoggedAltFunctorWithoutResult<T1, T2> Run(IMonitorLogger logger, string functorId, T1 arg1, T2 arg2, Action<IMonitorLogger, T1, T2> action, ICustomThreadPool threadPool, ICancellationContext cancellationContext, ISerializationAnchor serializationAnchor)
        {
            var functor = new LoggedAltFunctorWithoutResult<T1, T2>(logger, functorId, arg1, arg2, action, threadPool, cancellationContext, serializationAnchor);
            functor.Run();
            return functor;
        }

        public LoggedAltFunctorWithoutResult(IMonitorLogger logger, string functorId, T1 arg1, T2 arg2, Action<IMonitorLogger, T1, T2> action, ICustomThreadPool threadPool, ICancellationContext cancellationContext, ISerializationAnchor serializationAnchor)
            : base(logger, functorId, logger, arg1, arg2, action, threadPool, cancellationContext, serializationAnchor)
        {
        }
    }

    public class LoggedAltFunctorWithoutResult<T1, T2, T3> : BaseAltFunctorWithoutResult<IMonitorLogger, T1, T2, T3>
    {
        public static LoggedAltFunctorWithoutResult<T1, T2, T3> Run(IMonitorLogger logger, string functorId, T1 arg1, T2 arg2, T3 arg3, Action<IMonitorLogger, T1, T2, T3> action, ICustomThreadPool threadPool, ICancellationContext cancellationContext, ISerializationAnchor serializationAnchor)
        {
            var functor = new LoggedAltFunctorWithoutResult<T1, T2, T3>(logger, functorId, arg1, arg2, arg3, action, threadPool, cancellationContext, serializationAnchor);
            functor.Run();
            return functor;
        }

        public LoggedAltFunctorWithoutResult(IMonitorLogger logger, string functorId, T1 arg1, T2 arg2, T3 arg3, Action<IMonitorLogger, T1, T2, T3> action, ICustomThreadPool threadPool, ICancellationContext cancellationContext, ISerializationAnchor serializationAnchor)
            : base(logger, functorId, logger, arg1, arg2, arg3, action, threadPool, cancellationContext, serializationAnchor)
        {
        }
    }

    public class LoggedAltFunctorWithoutResult<T1, T2, T3, T4> : BaseAltFunctorWithoutResult<IMonitorLogger, T1, T2, T3, T4>
    {
        public static LoggedAltFunctorWithoutResult<T1, T2, T3, T4> Run(IMonitorLogger logger, string functorId, T1 arg1, T2 arg2, T3 arg3, T4 arg4, Action<IMonitorLogger, T1, T2, T3, T4> action, ICustomThreadPool threadPool, ICancellationContext cancellationContext, ISerializationAnchor serializationAnchor)
        {
            var functor = new LoggedAltFunctorWithoutResult<T1, T2, T3, T4>(logger, functorId, arg1, arg2, arg3, arg4, action, threadPool, cancellationContext, serializationAnchor);
            functor.Run();
            return functor;
        }

        public LoggedAltFunctorWithoutResult(IMonitorLogger logger, string functorId, T1 arg1, T2 arg2, T3 arg3, T4 arg4, Action<IMonitorLogger, T1, T2, T3, T4> action, ICustomThreadPool threadPool, ICancellationContext cancellationContext, ISerializationAnchor serializationAnchor)
            : base(logger, functorId, logger, arg1, arg2, arg3, arg4, action, threadPool, cancellationContext, serializationAnchor)
        {
        }
    }
}
