﻿using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.Threads;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Threading;
using System.Threading;

namespace SymOntoClay.Core.Internal.Serialization.Functors
{
    public abstract class BaseFunctor
    {
        protected BaseFunctor(IMonitorLogger logger, IActiveObjectContext context, ICustomThreadPool threadPool)
        {
            _asyncActiveOnceObject = new AsyncActiveOnceObject(context, threadPool, logger)
            {
                OnceMethod = OnRun
            };

            _asyncActiveOnceObject.OnCompleted += OnCompletedHandler;
        }

        private AsyncActiveOnceObject _asyncActiveOnceObject;

        public IThreadTask TaskValue => _asyncActiveOnceObject?.TaskValue; 

        protected abstract void OnRun(CancellationToken cancellationToken);

        public void Run()
        {
            _asyncActiveOnceObject.Start();
        }

        private void OnCompletedHandler()
        {
            _asyncActiveOnceObject.OnCompleted -= OnCompletedHandler;
            _asyncActiveOnceObject.Dispose();
            _asyncActiveOnceObject = null;
        }
    }

    public abstract class BaseFunctor<TResult>
    {
        protected BaseFunctor(IMonitorLogger logger, IActiveObjectContext context, ICustomThreadPool threadPool)
        {
            _asyncActiveOnceObject = new AsyncActiveOnceObject<TResult>(context, threadPool, logger)
            {
                OnceMethod = OnRun
            };

            _asyncActiveOnceObject.OnCompleted += OnCompletedHandler;
        }

        private AsyncActiveOnceObject<TResult> _asyncActiveOnceObject;

        public IThreadTask<TResult> TaskValue => _asyncActiveOnceObject?.TaskValueWithResult;

        protected abstract TResult OnRun(CancellationToken cancellationToken);

        public void Run()
        {
            _asyncActiveOnceObject.Start();
        }

        private void OnCompletedHandler()
        {
            _result = _asyncActiveOnceObject.Result;
            _asyncActiveOnceObject.OnCompleted -= OnCompletedHandler;
            _asyncActiveOnceObject.Dispose();
            _asyncActiveOnceObject = null;
        }

        private TResult _result = default(TResult);

        public TResult Result => _asyncActiveOnceObject == null ? _result : _asyncActiveOnceObject.Result;
    }
}
