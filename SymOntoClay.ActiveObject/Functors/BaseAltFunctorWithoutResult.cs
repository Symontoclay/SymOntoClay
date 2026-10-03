using SymOntoClay.ActiveObject.Threads;
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

    public class BaseAltFunctorWithoutResult<T1, T2> : BaseAltFunctor
    {
        public BaseAltFunctorWithoutResult(IMonitorLogger logger, string functorId, T1 arg1, T2 arg2, Action<T1, T2> action, ICustomThreadPool threadPool, ICancellationContext cancellationContext, ISerializationAnchor serializationAnchor)
            : base(logger, threadPool, cancellationContext, serializationAnchor)
        {
            _functorId = functorId;

            _action = action;
            _arg1 = arg1;
            _arg2 = arg2;
        }

        private string _functorId;

        [SerializedActionMember(nameof(_functorId), 0)]
        private Action<T1, T2> _action;
        private T1 _arg1;
        private T2 _arg2;

        /// <inheritdoc/>
        protected override void OnRun(ICancellationContext cancellationContext)
        {
            _action(_arg1, _arg2);
        }
    }

    public class BaseAltFunctorWithoutResult<T1, T2, T3> : BaseAltFunctor
    {
        public BaseAltFunctorWithoutResult(IMonitorLogger logger, string functorId, T1 arg1, T2 arg2, T3 arg3, Action<T1, T2, T3> action, ICustomThreadPool threadPool, ICancellationContext cancellationContext, ISerializationAnchor serializationAnchor)
            : base(logger, threadPool, cancellationContext, serializationAnchor)
        {
            _functorId = functorId;

            _action = action;
            _arg1 = arg1;
            _arg2 = arg2;
            _arg3 = arg3;
        }

        private string _functorId;

        [SerializedActionMember(nameof(_functorId), 0)]
        private Action<T1, T2, T3> _action;
        private T1 _arg1;
        private T2 _arg2;
        private T3 _arg3;

        /// <inheritdoc/>
        protected override void OnRun(ICancellationContext cancellationContext)
        {
            _action(_arg1, _arg2, _arg3);
        }
    }

    public class BaseAltFunctorWithoutResult<T1, T2, T3, T4> : BaseAltFunctor
    {
        public BaseAltFunctorWithoutResult(IMonitorLogger logger, string functorId, T1 arg1, T2 arg2, T3 arg3, T4 arg4, Action<T1, T2, T3, T4> action, ICustomThreadPool threadPool, ICancellationContext cancellationContext, ISerializationAnchor serializationAnchor)
            : base(logger, threadPool, cancellationContext, serializationAnchor)
        {
            _functorId = functorId;

            _action = action;
            _arg1 = arg1;
            _arg2 = arg2;
            _arg3 = arg3;
            _arg4 = arg4;
        }

        private string _functorId;

        [SerializedActionMember(nameof(_functorId), 0)]
        private Action<T1, T2, T3, T4> _action;
        private T1 _arg1;
        private T2 _arg2;
        private T3 _arg3;
        private T4 _arg4;

        /// <inheritdoc/>
        protected override void OnRun(ICancellationContext cancellationContext)
        {
            _action(_arg1, _arg2, _arg3, _arg4);
        }
    }

    public class BaseAltFunctorWithoutResult<T1, T2, T3, T4, T5> : BaseAltFunctor
    {
        public BaseAltFunctorWithoutResult(IMonitorLogger logger, string functorId, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, Action<T1, T2, T3, T4, T5> action, ICustomThreadPool threadPool, ICancellationContext cancellationContext, ISerializationAnchor serializationAnchor)
            : base(logger, threadPool, cancellationContext, serializationAnchor)
        {
            _functorId = functorId;

            _action = action;
            _arg1 = arg1;
            _arg2 = arg2;
            _arg3 = arg3;
            _arg4 = arg4;
            _arg5 = arg5;
        }

        private string _functorId;

        [SerializedActionMember(nameof(_functorId), 0)]
        private Action<T1, T2, T3, T4, T5> _action;
        private T1 _arg1;
        private T2 _arg2;
        private T3 _arg3;
        private T4 _arg4;
        private T5 _arg5;

        /// <inheritdoc/>
        protected override void OnRun(ICancellationContext cancellationContext)
        {
            _action(_arg1, _arg2, _arg3, _arg4, _arg5);
        }
    }
}
