using SymOntoClay.Core.Internal.CodeModel;
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
        }

        private readonly AsyncActiveOnceObject _asyncActiveOnceObject;

        protected abstract void OnRun(CancellationToken cancellationToken);

        public void Run()
        {
            _asyncActiveOnceObject.Start();
        }

        protected void DisposeActiveObject()
        {
            _asyncActiveOnceObject.Dispose();
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
        }

        private readonly AsyncActiveOnceObject<TResult> _asyncActiveOnceObject;

        protected abstract void OnRun(CancellationToken cancellationToken);

        public void Run()
        {
            _asyncActiveOnceObject.Start();
        }

        protected void DisposeActiveObject()
        {
            _asyncActiveOnceObject.Dispose();
        }
    }
}
