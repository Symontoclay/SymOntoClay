using SymOntoClay.ActiveObject.MethodResponses;
using SymOntoClay.ActiveObject.Threads;
using SymOntoClay.Monitor.Common;
using System.Threading;

namespace SymOntoClay.ActiveObject.Functors
{
    public abstract partial class BaseSyncFunctorWithoutResult : ISyncOnceObject, IBaseFunctor
    {
        protected BaseSyncFunctorWithoutResult(IMonitorLogger logger, IActiveObjectContext activeObjectContext, ISerializationAnchor serializationAnchor)
        {
            _serializationAnchor = serializationAnchor;
            serializationAnchor.AddFunctor(this);

            _activeObjectContext = activeObjectContext;
            activeObjectContext.AddChild(this);

            _cancellationToken = activeObjectContext.Token;

            _logger = logger;
        }

        protected abstract void OnRun();

        private IActiveObjectContext _activeObjectContext;
        private ISerializationAnchor _serializationAnchor;
        private readonly CancellationToken _cancellationToken;

        private bool _isWaited;

        /// <inheritdoc/>
        public bool IsWaited => _isWaited;

        /// <inheritdoc/>
        public bool IsActive => !_isFinished && !_isWaited;

        private IMonitorLogger _logger;

        private bool _isFinished;

        public void Run()
        {
            if (_isFinished)
            {
                return;
            }

            _isWaited = false;

            var autoResetEvent = _activeObjectContext.WaitEvent;

            if (_cancellationToken.IsCancellationRequested)
            {
                return;
            }

            if (_activeObjectContext.IsNeedWating)
            {
                _isWaited = true;
                autoResetEvent.WaitOne();
                _isWaited = false;
            }

            if (_cancellationToken.IsCancellationRequested)
            {
                return;
            }

            OnRun();

            _isFinished = true;

            _serializationAnchor.RemoveFunctor(this);
            _activeObjectContext.RemoveChild(this);
        }

        public IMethodResponse ToMethodResponse()
        {
            return CompletedMethodResponse.Instance;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            if (_isFinished)
            {
                return;
            }

            _isFinished = true;

            _serializationAnchor.RemoveFunctor(this);
            _activeObjectContext.RemoveChild(this);
        }
    }
}
