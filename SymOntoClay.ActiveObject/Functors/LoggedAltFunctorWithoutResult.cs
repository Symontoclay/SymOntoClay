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
}
