using SymOntoClay.ActiveObject.MethodResponses;
using SymOntoClay.ActiveObject.Threads;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Serialization;
using System;

namespace SymOntoClay.ActiveObject.Functors
{
    public abstract class SyncFunctorWithoutResult : BaseSyncFunctorWithoutResult
    {
        protected SyncFunctorWithoutResult(IMonitorLogger logger, string functorId, Action action, IActiveObjectContext activeObjectContext, ISerializationAnchor serializationAnchor)
            : base(logger, activeObjectContext, serializationAnchor)
        {
            _functorId = functorId;

            _action = action;
        }

        [SocSerializableActionKey]
        private string _functorId;

        private Action _action;

        /// <inheritdoc/>
        protected override void OnRun()
        {
            _action();
        }
    }

    public abstract class SyncFunctorWithoutResult<T> : BaseSyncFunctorWithoutResult
    {
        protected SyncFunctorWithoutResult(IMonitorLogger logger, string functorId, T arg, Action<T> action, IActiveObjectContext activeObjectContext, ISerializationAnchor serializationAnchor)
            : base(logger, activeObjectContext, serializationAnchor)
        {
            _functorId = functorId;

            _action = action;
            _arg = arg;
        }

        [SocSerializableActionKey]
        private string _functorId;

        private Action<T> _action;

        private T _arg;

        /// <inheritdoc/>
        protected override void OnRun()
        {
            _action(_arg);
        }
    }

    public abstract class SyncFunctorWithoutResult<T1, T2> : BaseSyncFunctorWithoutResult
    {
        protected SyncFunctorWithoutResult(IMonitorLogger logger, string functorId, T1 arg1, T2 arg2, Action<T1, T2> action, IActiveObjectContext activeObjectContext, ISerializationAnchor serializationAnchor)
            : base(logger, activeObjectContext, serializationAnchor)
        {
            _functorId = functorId;

            _action = action;
            _arg1 = arg1;
            _arg2 = arg2;
        }

        [SocSerializableActionKey]
        private string _functorId;

        private Action<T1, T2> _action;

        private T1 _arg1;
        private T2 _arg2;

        /// <inheritdoc/>
        protected override void OnRun()
        {
            _action(_arg1, _arg2);
        }
    }

    public abstract class SyncFunctorWithoutResult<T1, T2, T3> : BaseSyncFunctorWithoutResult
    {
        protected SyncFunctorWithoutResult(IMonitorLogger logger, string functorId, T1 arg1, T2 arg2, T3 arg3, Action<T1, T2, T3> action, IActiveObjectContext activeObjectContext, ISerializationAnchor serializationAnchor)
            : base(logger, activeObjectContext, serializationAnchor)
        {
            _functorId = functorId;

            _action = action;
            _arg1 = arg1;
            _arg2 = arg2;
            _arg3 = arg3;
        }

        [SocSerializableActionKey]
        private string _functorId;

        private Action<T1, T2, T3> _action;

        private T1 _arg1;
        private T2 _arg2;
        private T3 _arg3;

        /// <inheritdoc/>
        protected override void OnRun()
        {
            _action(_arg1, _arg2, _arg3);
        }
    }

    public abstract class SyncFunctorWithoutResult<T1, T2, T3, T4> : BaseSyncFunctorWithoutResult
    {
        protected SyncFunctorWithoutResult(IMonitorLogger logger, string functorId, T1 arg1, T2 arg2, T3 arg3, T4 arg4, Action<T1, T2, T3, T4> action, IActiveObjectContext activeObjectContext, ISerializationAnchor serializationAnchor)
            : base(logger, activeObjectContext, serializationAnchor)
        {
            _functorId = functorId;

            _action = action;
            _arg1 = arg1;
            _arg2 = arg2;
            _arg3 = arg3;
            _arg4 = arg4;
        }

        [SocSerializableActionKey]
        private string _functorId;

        private Action<T1, T2, T3, T4> _action;

        private T1 _arg1;
        private T2 _arg2;
        private T3 _arg3;
        private T4 _arg4;

        /// <inheritdoc/>
        protected override void OnRun()
        {
            _action(_arg1, _arg2, _arg3, _arg4);
        }
    }
}
