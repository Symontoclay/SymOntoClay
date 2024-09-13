using SymOntoClay.ActiveObject.Threads;
using SymOntoClay.Monitor.Common;
using System;

namespace SymOntoClay.ActiveObject.Functors
{
    public partial class LoggedSyncFunctorWithResult<TResult> : SyncFunctorWithResult<IMonitorLogger, TResult>
    {
        public static LoggedSyncFunctorWithResult<TResult> Run(IMonitorLogger logger, string functorId, Func<IMonitorLogger, TResult> func, IActiveObjectContext activeObjectContext, ISerializationAnchor serializationAnchor)
        {
            var functor = new LoggedSyncFunctorWithResult<TResult>(logger, functorId, func, activeObjectContext, serializationAnchor);
            functor.Run();
            return functor;
        }

        public LoggedSyncFunctorWithResult(IMonitorLogger logger, string functorId, Func<IMonitorLogger, TResult> func, IActiveObjectContext activeObjectContext, ISerializationAnchor serializationAnchor)
            : base(logger, functorId, logger, func, activeObjectContext, serializationAnchor)
        {
        }
    }

    public partial class LoggedSyncFunctorWithResult<T, TResult> : SyncFunctorWithResult<IMonitorLogger, T, TResult>
    {
        public static LoggedSyncFunctorWithResult<T, TResult> Run(IMonitorLogger logger, string functorId, T arg, Func<IMonitorLogger, T, TResult> func, IActiveObjectContext activeObjectContext, ISerializationAnchor serializationAnchor)
        {
            var functor = new LoggedSyncFunctorWithResult<T, TResult>(logger, functorId, arg, func, activeObjectContext, serializationAnchor);
            functor.Run();
            return functor;
        }

        public LoggedSyncFunctorWithResult(IMonitorLogger logger, string functorId, T arg, Func<IMonitorLogger, T, TResult> func, IActiveObjectContext activeObjectContext, ISerializationAnchor serializationAnchor)
            : base(logger, functorId, logger, arg, func, activeObjectContext, serializationAnchor)
        {
        }
    }

    public partial class LoggedSyncFunctorWithResult<T1, T2, TResult> : SyncFunctorWithResult<IMonitorLogger, T1, T2, TResult>
    {
        public static LoggedSyncFunctorWithResult<T1, T2, TResult> Run(IMonitorLogger logger, string functorId, T1 arg1, T2 arg2, Func<IMonitorLogger, T1, T2, TResult> func, IActiveObjectContext activeObjectContext, ISerializationAnchor serializationAnchor)
        {
            var functor = new LoggedSyncFunctorWithResult<T1, T2, TResult>(logger, functorId, arg1, arg2, func, activeObjectContext, serializationAnchor);
            functor.Run();
            return functor;
        }

        public LoggedSyncFunctorWithResult(IMonitorLogger logger, string functorId, T1 arg1, T2 arg2, Func<IMonitorLogger, T1, T2, TResult> func, IActiveObjectContext activeObjectContext, ISerializationAnchor serializationAnchor)
            : base(logger, functorId, logger, arg1, arg2, func, activeObjectContext, serializationAnchor)
        {
        }
    }
}
