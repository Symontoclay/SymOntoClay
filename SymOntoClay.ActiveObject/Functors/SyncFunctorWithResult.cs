using SymOntoClay.ActiveObject.MethodResponses;
using SymOntoClay.ActiveObject.Threads;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Serialization;
using System;

namespace SymOntoClay.ActiveObject.Functors
{
    public abstract class SyncFunctorWithResult<TResult> : BaseSyncFunctorWithResult<TResult>
    {
        protected SyncFunctorWithResult(IMonitorLogger logger, string functorId, Func<TResult> func, IActiveObjectContext activeObjectContext, ISerializationAnchor serializationAnchor)
            : base(logger, activeObjectContext, serializationAnchor)
        {
            _functorId = functorId;

            _func = func;
        }

        private string _functorId;

        [SocSerializableActionMember(nameof(_functorId), 0)]
        private Func<TResult> _func;

        /// <inheritdoc/>
        protected override TResult OnRun()
        {
            return _func();
        }
    }

    public abstract class SyncFunctorWithResult<T, TResult> : BaseSyncFunctorWithResult<TResult>
    {
        protected SyncFunctorWithResult(IMonitorLogger logger, string functorId, T arg, Func<T, TResult> func, IActiveObjectContext activeObjectContext, ISerializationAnchor serializationAnchor)
            : base(logger, activeObjectContext, serializationAnchor)
        {
            _functorId = functorId;

            _func = func;
            _arg = arg;
        }

        private string _functorId;

        [SocSerializableActionMember(nameof(_functorId), 0)]
        private Func<T, TResult> _func;
        private T _arg;

        /// <inheritdoc/>
        protected override TResult OnRun()
        {
            return _func(_arg);
        }
    }

    public abstract class SyncFunctorWithResult<T1, T2, TResult> : BaseSyncFunctorWithResult<TResult>
    {
        protected SyncFunctorWithResult(IMonitorLogger logger, string functorId, T1 arg1, T2 arg2, Func<T1, T2, TResult> func, IActiveObjectContext activeObjectContext, ISerializationAnchor serializationAnchor)
            : base(logger, activeObjectContext, serializationAnchor)
        {
            _functorId = functorId;

            _func = func;
            _arg1 = arg1;
            _arg2 = arg2;
        }

        private string _functorId;

        [SocSerializableActionMember(nameof(_functorId), 0)]
        private Func<T1, T2, TResult> _func;

        private T1 _arg1;
        private T2 _arg2;

        /// <inheritdoc/>
        protected override TResult OnRun()
        {
            return _func(_arg1, _arg2);
        }
    }

    public abstract class SyncFunctorWithResult<T1, T2, T3, TResult> : BaseSyncFunctorWithResult<TResult>
    {
        protected SyncFunctorWithResult(IMonitorLogger logger, string functorId, T1 arg1, T2 arg2, T3 arg3, Func<T1, T2, T3, TResult> func, IActiveObjectContext activeObjectContext, ISerializationAnchor serializationAnchor)
            : base(logger, activeObjectContext, serializationAnchor)
        {
            _functorId = functorId;

            _func = func;
            _arg1 = arg1;
            _arg2 = arg2;
            _arg3 = arg3;
        }

        private string _functorId;

        [SocSerializableActionMember(nameof(_functorId), 0)]
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

    public abstract class SyncFunctorWithResult<T1, T2, T3, T4, TResult> : BaseSyncFunctorWithResult<TResult>
    {
        protected SyncFunctorWithResult(IMonitorLogger logger, string functorId, T1 arg1, T2 arg2, T3 arg3, T4 arg4, Func<T1, T2, T3, T4, TResult> func, IActiveObjectContext activeObjectContext, ISerializationAnchor serializationAnchor)
            : base(logger, activeObjectContext, serializationAnchor)
        {
            _functorId = functorId;

            _func = func;
            _arg1 = arg1;
            _arg2 = arg2;
            _arg3 = arg3;
            _arg4 = arg4;
        }

        private string _functorId;

        [SocSerializableActionMember(nameof(_functorId), 0)]
        private Func<T1, T2, T3, T4, TResult> _func;

        private T1 _arg1;
        private T2 _arg2;
        private T3 _arg3;
        private T4 _arg4;

        /// <inheritdoc/>
        protected override TResult OnRun()
        {
            return _func(_arg1, _arg2, _arg3, _arg4);
        }
    }
}
