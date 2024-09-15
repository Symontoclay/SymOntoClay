using SymOntoClay.ActiveObject.MethodResponses;
using SymOntoClay.ActiveObject.Threads;
using SymOntoClay.Monitor.Common;
using System.Threading;

namespace SymOntoClay.ActiveObject.Functors
{
    public abstract partial class BaseSyncFunctorWithResult<TResult> : ISyncOnceObject, IBaseFunctor
    {
        protected BaseSyncFunctorWithResult(IMonitorLogger logger, IActiveObjectContext activeObjectContext, ISerializationAnchor serializationAnchor) 
        {
            _serializationAnchor = serializationAnchor;
            serializationAnchor.AddFunctor(this);

            _activeObjectContext = activeObjectContext;
            activeObjectContext.AddChild(this);

            _cancellationToken = activeObjectContext.Token;

            _logger = logger;
        }

        protected abstract TResult OnRun();

        private IActiveObjectContext _activeObjectContext;
        private ISerializationAnchor _serializationAnchor;
        private readonly CancellationToken _cancellationToken;

        private IMonitorLogger _logger;

        private bool _isWaited;

        /// <inheritdoc/>
        public bool IsWaited => _isWaited;

        /// <inheritdoc/>
        public bool IsActive => !_isFinished && !_isWaited;

        private bool _isFinished;

        private TResult _result;

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

            _result = OnRun();

            _isFinished = true;

            _serializationAnchor.RemoveFunctor(this);
            _activeObjectContext.RemoveChild(this);
        }

        public ISyncMethodResponse<TResult> ToMethodResponse()
        {
            return new CompletedSyncMethodResponse<TResult>(_result);
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
