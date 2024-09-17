using SymOntoClay.ActiveObject.EventsInterfaces;
using SymOntoClay.ActiveObject.MethodResponses;
using SymOntoClay.ActiveObject.Threads;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Threading;
using System.Threading;

namespace SymOntoClay.ActiveObject.Functors
{
    public abstract class BaseFunctor: IBaseFunctor, IOnCompletedAsyncActiveOnceObjectHandler
    {
        protected BaseFunctor(IMonitorLogger logger, IActiveObjectContext context, ICustomThreadPool threadPool, ISerializationAnchor serializationAnchor)
        {
            _serializationAnchor = serializationAnchor;
            serializationAnchor.AddFunctor(this);

            _asyncActiveOnceObject = new AsyncActiveOnceObject(context, threadPool, logger)
            {
                OnceMethod = OnRun
            };

            _asyncActiveOnceObject.AddOnCompletedHandler(this);
        }

        private AsyncActiveOnceObject _asyncActiveOnceObject;
        private ISerializationAnchor _serializationAnchor;

        public IThreadTask TaskValue => _asyncActiveOnceObject?.TaskValue;

        protected abstract void OnRun(CancellationToken cancellationToken);

        public void Run()
        {
            _asyncActiveOnceObject.Start();
        }

        void IOnCompletedAsyncActiveOnceObjectHandler.Invoke()
        {
            OnCompletedHandler();
        }

        private void OnCompletedHandler()
        {
            _serializationAnchor.RemoveFunctor(this);
            _asyncActiveOnceObject.RemoveOnCompletedHandler(this);
            _asyncActiveOnceObject.Dispose();
            _asyncActiveOnceObject = null;
        }

        public IAsyncMethodResponse ToMethodResponse()
        {
            return new AsyncMethodResponseOfBaseFunctor(this);
        }
    }

    public abstract class BaseFunctor<TResult> : IBaseFunctor, IOnCompletedAsyncActiveOnceObjectHandler
    {
        protected BaseFunctor(IMonitorLogger logger, IActiveObjectContext context, ICustomThreadPool threadPool, ISerializationAnchor serializationAnchor)
        {
            _serializationAnchor = serializationAnchor;
            serializationAnchor.AddFunctor(this);

            _asyncActiveOnceObject = new AsyncActiveOnceObject<TResult>(context, threadPool, logger)
            {
                OnceMethod = OnRun
            };

            _asyncActiveOnceObject.AddOnCompletedHandler(this);
        }

        private AsyncActiveOnceObject<TResult> _asyncActiveOnceObject;

        private ISerializationAnchor _serializationAnchor;

        public IThreadTask<TResult> TaskValue => _asyncActiveOnceObject?.TaskValueWithResult;

        protected abstract TResult OnRun(CancellationToken cancellationToken);

        public void Run()
        {
            _asyncActiveOnceObject.Start();
        }

        void IOnCompletedAsyncActiveOnceObjectHandler.Invoke()
        {
            OnCompletedHandler();
        }

        private void OnCompletedHandler()
        {
            _serializationAnchor.RemoveFunctor(this);
            _result = _asyncActiveOnceObject.Result;
            _asyncActiveOnceObject.RemoveOnCompletedHandler(this);
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
