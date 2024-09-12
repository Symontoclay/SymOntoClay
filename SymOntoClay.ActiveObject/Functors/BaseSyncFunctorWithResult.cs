using SymOntoClay.ActiveObject.MethodResponses;
using SymOntoClay.Monitor.Common;

namespace SymOntoClay.ActiveObject.Functors
{
    public abstract partial class BaseSyncFunctorWithResult<TResult> : IBaseFunctor
    {
        protected BaseSyncFunctorWithResult(IMonitorLogger logger, ISerializationAnchor serializationAnchor) 
        {
            _serializationAnchor = serializationAnchor;
            serializationAnchor.AddFunctor(this);

            _logger = logger;
        }

        protected abstract TResult OnRun();

        private ISerializationAnchor _serializationAnchor;

        private IMonitorLogger _logger;

        private bool _isFinished;

        private TResult _result;

        public void Run()
        {
            if (_isFinished)
            {
                return;
            }

            _result = OnRun();

            _isFinished = true;

            _serializationAnchor.RemoveFunctor(this);
        }

        public IMethodResponse<TResult> ToMethodResponse()
        {
            return new CompletedMethodResponse<TResult>(_result);
        }
    }
}
