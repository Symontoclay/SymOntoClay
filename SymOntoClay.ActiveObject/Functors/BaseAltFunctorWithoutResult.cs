using SymOntoClay.Common.Cancellation;
using SymOntoClay.CoreHelper.SerializationToImage.Attributes;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Threading;
using System;

namespace SymOntoClay.ActiveObject.Functors
{
    public class BaseAltFunctorWithoutResult : BaseAltFunctor
    {
        public BaseAltFunctorWithoutResult(IMonitorLogger logger, string functorId, Action action, ICustomThreadPool threadPool, ICancellationContext cancellationContext, ISerializationAnchor serializationAnchor)
            : base(logger, threadPool, cancellationContext, serializationAnchor)
        {
            _functorId = functorId;

            _action = action;
        }

        private string _functorId;

        [SerializedActionMember(nameof(_functorId), 0)]
        private Action _action;

        /// <inheritdoc/>
        protected override void OnRun(ICancellationContext cancellationContext)
        {
            _action();
        }
    }

    public class BaseAltFunctorWithoutResult<T>: BaseAltFunctor
    {
        public BaseAltFunctorWithoutResult(IMonitorLogger logger, string functorId, T arg, Action<T> action, ICustomThreadPool threadPool, ICancellationContext cancellationContext, ISerializationAnchor serializationAnchor)
            : base(logger, threadPool, cancellationContext, serializationAnchor)
        {
            _functorId = functorId;

            _action = action;
            _arg = arg;
        }

        private string _functorId;

        [SerializedActionMember(nameof(_functorId), 0)]
        private Action<T> _action;
        private T _arg;

        /// <inheritdoc/>
        protected override void OnRun(ICancellationContext cancellationContext)
        {
            _action(_arg);
        }
    }
}
