using SymOntoClay.ActiveObject.MethodResponses;
using SymOntoClay.ActiveObject.Threads;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Threading;
using System.Threading;

namespace SymOntoClay.ActiveObject.Functors
{
    public abstract class BaseFunctor: IBaseFunctor
    {
        protected BaseFunctor(IMonitorLogger logger, IActiveObjectContext context, ICustomThreadPool threadPool, ISerializationAnchor serializationAnchor)
        {
            _serializationAnchor = serializationAnchor;
            serializationAnchor.AddFunctor(this);

            _asyncActiveOnceObject = new AsyncActiveOnceObject(context, threadPool, logger)
            {
                OnceMethod = OnRun
            };

            _asyncActiveOnceObject.OnCompleted += OnCompletedHandler;
        }

        private AsyncActiveOnceObject _asyncActiveOnceObject;
        private ISerializationAnchor _serializationAnchor;

        public IThreadTask TaskValue => _asyncActiveOnceObject?.TaskValue;

        protected abstract void OnRun(CancellationToken cancellationToken);

        public void Run()
        {
            _asyncActiveOnceObject.Start();
        }

        private void OnCompletedHandler()
        {
            _serializationAnchor.RemoveFunctor(this);
            _asyncActiveOnceObject.OnCompleted -= OnCompletedHandler;
            _asyncActiveOnceObject.Dispose();
            _asyncActiveOnceObject = null;
        }

        public IAsyncMethodResponse ToMethodResponse()
        {
            return new AsyncMethodResponseOfBaseFunctor(this);
        }
    }

    public abstract class BaseFunctor<TResult> : IBaseFunctor
    {
        protected BaseFunctor(IMonitorLogger logger, IActiveObjectContext context, ICustomThreadPool threadPool, ISerializationAnchor serializationAnchor)
        {
            _serializationAnchor = serializationAnchor;
            serializationAnchor.AddFunctor(this);

            _asyncActiveOnceObject = new AsyncActiveOnceObject<TResult>(context, threadPool, logger)
            {
                OnceMethod = OnRun
            };

            _asyncActiveOnceObject.OnCompleted += OnCompletedHandler;
        }

        private AsyncActiveOnceObject<TResult> _asyncActiveOnceObject;

        private ISerializationAnchor _serializationAnchor;

        public IThreadTask<TResult> TaskValue => _asyncActiveOnceObject?.TaskValueWithResult;

        protected abstract TResult OnRun(CancellationToken cancellationToken);

        public void Run()
        {
            _asyncActiveOnceObject.Start();
        }

        private void OnCompletedHandler()
        {
            _serializationAnchor.RemoveFunctor(this);
            _result = _asyncActiveOnceObject.Result;
            _asyncActiveOnceObject.OnCompleted -= OnCompletedHandler;
            _asyncActiveOnceObject.Dispose();
            _asyncActiveOnceObject = null;
        }

        private TResult _result = default(TResult);

        public TResult Result => _asyncActiveOnceObject == null ? _result : _asyncActiveOnceObject.Result;

        public IAsyncMethodResponse<TResult> ToMethodResponse()
        {
            return new AsyncMethodResponseOfBaseFunctor<TResult>(this);
        }
    }
}
