using SymOntoClay.ActiveObject.EventsInterfaces;
using SymOntoClay.ActiveObject.Pointers;
using SymOntoClay.ActiveObject.Threads;
using SymOntoClay.Common.Cancellation;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Threading;
using System.Threading;

namespace SymOntoClay.ActiveObject.Functors
{
    public abstract class BaseFunctor : IBaseFunctor, 
        IOnCompletedActiveObjectHandler, IObjectWithOnceRunMethod
    {
        protected BaseFunctor(IMonitorLogger logger, IActiveObjectContext context, ICustomThreadPool threadPool, ISerializationAnchor serializationAnchor)
        {
            _serializationAnchor = serializationAnchor;
            serializationAnchor.AddFunctor(this);

            _asyncActiveOnceObject = new AsyncActiveOnceObject(context, threadPool, logger)
            {
                ObjectWithOnceRunMethod = this
            };

            _asyncActiveOnceObject.AddOnCompletedHandler(this);
        }

        private AsyncActiveOnceObject _asyncActiveOnceObject;
        private ISerializationAnchor _serializationAnchor;

        public IThreadTaskPointer TaskValue => _asyncActiveOnceObject?.TaskValue;

        void IObjectWithOnceRunMethod.OnceRunHandler(ICancellationContext cancellationContext)
        {
            OnRun(cancellationContext);
        }

        protected abstract void OnRun(ICancellationContext cancellationContext);

        /// <inheritdoc/>
        public void Run()
        {
            _asyncActiveOnceObject.Start();
        }

        void IOnCompletedActiveObjectHandler.Invoke()
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
    }

    public abstract class BaseFunctor<TResult> : IBaseFunctor,
        IOnCompletedActiveObjectHandler, IObjectWithOnceRunMethod<TResult>
    {
        protected BaseFunctor(IMonitorLogger logger, IActiveObjectContext context, ICustomThreadPool threadPool, ISerializationAnchor serializationAnchor)
        {
            _serializationAnchor = serializationAnchor;
            serializationAnchor.AddFunctor(this);

            _asyncActiveOnceObject = new AsyncActiveOnceObject<TResult>(context, threadPool, logger)
            {
                ObjectWithOnceRunMethod = this
            };

            _asyncActiveOnceObject.AddOnCompletedHandler(this);
        }

        private AsyncActiveOnceObject<TResult> _asyncActiveOnceObject;

        private ISerializationAnchor _serializationAnchor;

        public IThreadTaskPointer<TResult> TaskValue => _asyncActiveOnceObject?.TaskValueWithResult;

        TResult IObjectWithOnceRunMethod<TResult>.OnceRunHandler(ICancellationContext cancellationContext)
        {
            return OnRun(cancellationContext);
        }

        protected abstract TResult OnRun(ICancellationContext cancellationContext);

        /// <inheritdoc/>
        public void Run()
        {
            _asyncActiveOnceObject.Start();
        }

        void IOnCompletedActiveObjectHandler.Invoke()
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
    }
}
