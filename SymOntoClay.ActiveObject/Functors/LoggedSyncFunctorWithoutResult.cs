using SymOntoClay.ActiveObject.Threads;
using SymOntoClay.Monitor.Common;
using System;

namespace SymOntoClay.ActiveObject.Functors
{
    public partial class LoggedSyncFunctorWithoutResult: SyncFunctorWithoutResult<IMonitorLogger>
    {
        public static LoggedSyncFunctorWithoutResult Run(IMonitorLogger logger, string functorId, Action<IMonitorLogger> action, IActiveObjectContext activeObjectContext, ISerializationAnchor serializationAnchor)
        {
            var functor = new LoggedSyncFunctorWithoutResult(logger, functorId, action, activeObjectContext, serializationAnchor);
            functor.Run();
            return functor;
        }

        public LoggedSyncFunctorWithoutResult(IMonitorLogger logger, string functorId, Action<IMonitorLogger> action, IActiveObjectContext activeObjectContext, ISerializationAnchor serializationAnchor)
            : base(logger, functorId, logger, action, activeObjectContext, serializationAnchor)
        {
        }
    }

    public partial class LoggedSyncFunctorWithoutResult<T> : SyncFunctorWithoutResult<IMonitorLogger, T>
    {
        public static LoggedSyncFunctorWithoutResult<T> Run(IMonitorLogger logger, string functorId, T arg, Action<IMonitorLogger, T> action, IActiveObjectContext activeObjectContext, ISerializationAnchor serializationAnchor)
        {
            var functor = new LoggedSyncFunctorWithoutResult<T>(logger, functorId, arg, action, activeObjectContext, serializationAnchor);
            functor.Run();
            return functor;
        }

        public LoggedSyncFunctorWithoutResult(IMonitorLogger logger, string functorId, T arg, Action<IMonitorLogger, T> action, IActiveObjectContext activeObjectContext, ISerializationAnchor serializationAnchor)
            : base(logger, functorId, logger, arg, action, activeObjectContext, serializationAnchor)
        {
        }
    }

    public partial class LoggedSyncFunctorWithoutResult<T1, T2> : SyncFunctorWithoutResult<IMonitorLogger, T1, T2>
    {
        public static LoggedSyncFunctorWithoutResult<T1, T2> Run(IMonitorLogger logger, string functorId, T1 arg1, T2 arg2, Action<IMonitorLogger, T1, T2> action, IActiveObjectContext activeObjectContext, ISerializationAnchor serializationAnchor)
        {
            var functor = new LoggedSyncFunctorWithoutResult<T1, T2>(logger, functorId, arg1, arg2, action, activeObjectContext, serializationAnchor);
            functor.Run();
            return functor;
        }

        public LoggedSyncFunctorWithoutResult(IMonitorLogger logger, string functorId, T1 arg1, T2 arg2, Action<IMonitorLogger, T1, T2> action, IActiveObjectContext activeObjectContext, ISerializationAnchor serializationAnchor)
            : base(logger, functorId, logger, arg1, arg2, action, activeObjectContext, serializationAnchor)
        {
        }
    }

    public partial class LoggedSyncFunctorWithoutResult<T1, T2, T3> : SyncFunctorWithoutResult<IMonitorLogger, T1, T2, T3>
    {
        public static LoggedSyncFunctorWithoutResult<T1, T2, T3> Run(IMonitorLogger logger, string functorId, T1 arg1, T2 arg2, T3 arg3, Action<IMonitorLogger, T1, T2, T3> action, IActiveObjectContext activeObjectContext, ISerializationAnchor serializationAnchor)
        {
            var functor = new LoggedSyncFunctorWithoutResult<T1, T2, T3>(logger, functorId, arg1, arg2, arg3, action, activeObjectContext, serializationAnchor);
            functor.Run();
            return functor;
        }

        public LoggedSyncFunctorWithoutResult(IMonitorLogger logger, string functorId, T1 arg1, T2 arg2, T3 arg3, Action<IMonitorLogger, T1, T2, T3> action, IActiveObjectContext activeObjectContext, ISerializationAnchor serializationAnchor)
            : base(logger, functorId, logger, arg1, arg2, arg3, action, activeObjectContext, serializationAnchor)
        {
        }
    }
}
