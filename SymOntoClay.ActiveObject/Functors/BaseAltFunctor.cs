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
            _serializationAnchor = serializationAnchor;
            serializationAnchor.AddFunctor(this);


        }

        private ISerializationAnchor _serializationAnchor;

        protected abstract void OnRun(ICancellationContext cancellationContext);

        /// <inheritdoc/>
        public void Run()
        {
            throw new NotImplementedException("22CD1FEA-1301-4E80-BD45-65464F515FCA");
        }
    }
}
