using SymOntoClay.Monitor.Common;
using System;

namespace SymOntoClay.ActiveObject.Functors
{
    public partial class LoggedSyncFunctorWithoutResult: BaseSyncFunctorWithoutResult<IMonitorLogger>
    {
        public static LoggedSyncFunctorWithoutResult Run(IMonitorLogger logger, string functorId, Action<IMonitorLogger> action, ISerializationAnchor serializationAnchor)
        {
            var functor = new LoggedSyncFunctorWithoutResult(logger, functorId, action, serializationAnchor);
            functor.Run();
            return functor;
        }

        public LoggedSyncFunctorWithoutResult(IMonitorLogger logger, string functorId, Action<IMonitorLogger> action, ISerializationAnchor serializationAnchor)
            : base(logger, functorId, logger, action, serializationAnchor)
        {
        }
    }

    public partial class LoggedSyncFunctorWithoutResult<T> : BaseSyncFunctorWithoutResult<IMonitorLogger, T>
    {
        public static LoggedSyncFunctorWithoutResult<T> Run(IMonitorLogger logger, string functorId, T arg, Action<IMonitorLogger, T> action, ISerializationAnchor serializationAnchor)
        {
            var functor = new LoggedSyncFunctorWithoutResult<T>(logger, functorId, arg, action, serializationAnchor);
            functor.Run();
            return functor;
        }

        public LoggedSyncFunctorWithoutResult(IMonitorLogger logger, string functorId, T arg, Action<IMonitorLogger, T> action, ISerializationAnchor serializationAnchor)
            : base(logger, functorId, logger, arg, action, serializationAnchor)
        {
        }
    }

    public partial class LoggedSyncFunctorWithoutResult<T1, T2> : BaseSyncFunctorWithoutResult<IMonitorLogger, T1, T2>
    {
        public static LoggedSyncFunctorWithoutResult<T1, T2> Run(IMonitorLogger logger, string functorId, T1 arg1, T2 arg2, Action<IMonitorLogger, T1, T2> action, ISerializationAnchor serializationAnchor)
        {
            var functor = new LoggedSyncFunctorWithoutResult<T1, T2>(logger, functorId, arg1, arg2, action, serializationAnchor);
            functor.Run();
            return functor;
        }

        public LoggedSyncFunctorWithoutResult(IMonitorLogger logger, string functorId, T1 arg1, T2 arg2, Action<IMonitorLogger, T1, T2> action, ISerializationAnchor serializationAnchor)
            : base(logger, functorId, logger, arg1, arg2, action, serializationAnchor)
        {
        }
    }
}
