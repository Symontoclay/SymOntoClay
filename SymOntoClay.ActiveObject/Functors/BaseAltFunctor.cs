using SymOntoClay.ActiveObject.Threads;
using SymOntoClay.Common.Cancellation;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Threading;
using System;

namespace SymOntoClay.ActiveObject.Functors
{
    public abstract class BaseAltFunctor: IBaseFunctor
    {
        protected BaseAltFunctor(IMonitorLogger logger, ICustomThreadPool threadPool, ICancellationContext cancellationContext, ISerializationAnchor serializationAnchor)
        {
            _logger = logger;
            _threadPool = threadPool;
            _cancellationContext = cancellationContext;

            _serializationAnchor = serializationAnchor;
            serializationAnchor.AddFunctor(this);
        }

        private IMonitorLogger _logger;
        private ICustomThreadPool _threadPool; 
        private ICancellationContext _cancellationContext;
        private ISerializationAnchor _serializationAnchor;

        protected abstract void OnRun(ICancellationContext cancellationContext);

        /// <inheritdoc/>
        public void Run()
        {
            ThreadTask.Run(() => {
#if DEBUG
                //_logger.Info("2889715A-B9F4-46CB-8A70-0C2CB9B72E41", "Run!!!!!!!");
#endif

                OnRun(_cancellationContext);

                _serializationAnchor.RemoveFunctor(this);
            },
            _threadPool, _cancellationContext);
        }
    }
}
