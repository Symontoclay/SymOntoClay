using SymOntoClay.ActiveObject.Functors;
using SymOntoClay.ActiveObject.Threads;
using SymOntoClay.Core.EventsInterfaces;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Threading;

namespace SymOntoClay.Core.Internal.Instances
{
    public class BaseInstanceParentExecutionCoordinatorOnFinishedHandler : IOnFinishedExecutionCoordinatorHandler,
        IBaseInstanceParentExecutionCoordinatorOnFinishedHandlerSerializedEventsHandler
    {
        public BaseInstanceParentExecutionCoordinatorOnFinishedHandler(IMonitorLogger logger, IExecutionCoordinator executionCoordinator,
            IActiveObjectContext activeObjectContext, ICustomThreadPool threadPool, SerializationAnchor serializationAnchor)
        {
            _logger = logger;
            _executionCoordinator = executionCoordinator;
            _activeObjectContext = activeObjectContext;
            _threadPool = threadPool;
            _serializationAnchor = serializationAnchor;
        }

        private IMonitorLogger _logger;
        private IExecutionCoordinator _executionCoordinator;
        private IActiveObjectContext _activeObjectContext;
        private ICustomThreadPool _threadPool;
        private SerializationAnchor _serializationAnchor;

        void IOnFinishedExecutionCoordinatorHandler.Invoke()
        {
            LoggedFunctorWithoutResult<IBaseInstanceParentExecutionCoordinatorOnFinishedHandlerSerializedEventsHandler>.Run(_logger, "008333AA-D2FE-4DF7-8874-E0325A0767EA", this,
                (IMonitorLogger loggerValue, IBaseInstanceParentExecutionCoordinatorOnFinishedHandlerSerializedEventsHandler instanceValue) => {
                    instanceValue.NParentExecutionCoordinator_OnFinished();
                },
                _activeObjectContext, _threadPool, _serializationAnchor);
        }

        void IBaseInstanceParentExecutionCoordinatorOnFinishedHandlerSerializedEventsHandler.NParentExecutionCoordinator_OnFinished()
        {
            _executionCoordinator.SetExecutionStatus(_logger, "898C6E87-54F5-41CA-8B3F-08B8BBF95135", ActionExecutionStatus.WeakCanceled);
        }
    }
}
