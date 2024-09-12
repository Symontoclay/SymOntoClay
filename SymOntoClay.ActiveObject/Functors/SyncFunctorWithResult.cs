using SymOntoClay.ActiveObject.MethodResponses;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Serialization;
using System;

namespace SymOntoClay.ActiveObject.Functors
{
    public partial class SyncFunctorWithResult<TResult> : BaseSyncFunctorWithResult<TResult>
    {
        public SyncFunctorWithResult(IMonitorLogger logger, string functorId, Func<TResult> func, ISerializationAnchor serializationAnchor)
            : base(logger, serializationAnchor)
        {
            _functorId = functorId;

            _func = func;
        }

        [SocSerializableActionKey]
        private string _functorId;

        private Func<TResult> _func;

        /// <inheritdoc/>
        protected override TResult OnRun()
        {
            return _func();
        }
    }

    public partial class SyncFunctorWithResult<T, TResult> : BaseSyncFunctorWithResult<TResult>
    {
        public SyncFunctorWithResult(IMonitorLogger logger, string functorId, T arg, Func<T, TResult> func, ISerializationAnchor serializationAnchor)
            : base(logger, serializationAnchor)
        {
            _functorId = functorId;

            _func = func;
            _arg = arg;
        }

        [SocSerializableActionKey]
        private string _functorId;

        private Func<T, TResult> _func;
        private T _arg;

        /// <inheritdoc/>
        protected override TResult OnRun()
        {
            return _func(_arg);
        }
    }

    public partial class SyncFunctorWithResult<T1, T2, TResult> : BaseSyncFunctorWithResult<TResult>
    {
        public SyncFunctorWithResult(IMonitorLogger logger, string functorId, T1 arg1, T2 arg2, Func<T1, T2, TResult> func, ISerializationAnchor serializationAnchor)
            : base(logger, serializationAnchor)
        {
            _functorId = functorId;

            _func = func;
            _arg1 = arg1;
            _arg2 = arg2;
        }

        [SocSerializableActionKey]
        private string _functorId;

        private Func<T1, T2, TResult> _func;

        private T1 _arg1;
        private T2 _arg2;

        /// <inheritdoc/>
        protected override TResult OnRun()
        {
            return _func(_arg1, _arg2);
        }
    }

    public partial class SyncFunctorWithResult<T1, T2, T3, TResult> : BaseSyncFunctorWithResult<TResult>
    {
        public SyncFunctorWithResult(IMonitorLogger logger, string functorId, T1 arg1, T2 arg2, T3 arg3, Func<T1, T2, T3, TResult> func, ISerializationAnchor serializationAnchor)
            : base(logger, serializationAnchor)
        {
            _functorId = functorId;

            _func = func;
            _arg1 = arg1;
            _arg2 = arg2;
            _arg3 = arg3;
        }

        [SocSerializableActionKey]
        private string _functorId;

        private Func<T1, T2, T3, TResult> _func;

        private T1 _arg1;
        private T2 _arg2;
        private T3 _arg3;

        /// <inheritdoc/>
        protected override TResult OnRun()
        {
            return _func(_arg1, _arg2, _arg3);
        }
    }
}
