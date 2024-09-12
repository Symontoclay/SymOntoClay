using SymOntoClay.ActiveObject.MethodResponses;
using SymOntoClay.Monitor.Common;

namespace SymOntoClay.ActiveObject.Functors
{
    public abstract partial class BaseSyncFunctorWithoutResult : IBaseFunctor
    {
        protected BaseSyncFunctorWithoutResult(IMonitorLogger logger, ISerializationAnchor serializationAnchor)
        {
            _serializationAnchor = serializationAnchor;
            serializationAnchor.AddFunctor(this);

            _logger = logger;
        }

        protected abstract void OnRun();

        private ISerializationAnchor _serializationAnchor;

        private IMonitorLogger _logger;

        private bool _isFinished;

        public void Run()
        {
            if (_isFinished)
            {
                return;
            }

            OnRun();

            _isFinished = true;

            _serializationAnchor.RemoveFunctor(this);
        }

        public IMethodResponse ToMethodResponse()
        {
            return CompletedMethodResponse.Instance;
        }
    }
}
