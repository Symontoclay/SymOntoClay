using SymOntoClay.ActiveObject.Threads;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Threading;
using System.Threading;
using System;

namespace SymOntoClay.ActiveObject.Functors
{
    public class BaseFunctorWithoutResult : BaseFunctor
    {
        public BaseFunctorWithoutResult(IMonitorLogger logger, string functorId, Action action, IActiveObjectContext context, ICustomThreadPool threadPool, ISerializationAnchor serializationAnchor)
            : base(logger, context, threadPool, serializationAnchor)
        {
            _functorId = functorId;

            _action = action;
        }

        private string _functorId;

        //[SocSerializableActionMember(nameof(_functorId), 0)]
        private Action _action;

        /// <inheritdoc/>
        protected override void OnRun(CancellationToken cancellationToken)
        {
            _action();
        }
    }

    public class BaseFunctorWithoutResult<T> : BaseFunctor
    {
        public BaseFunctorWithoutResult(IMonitorLogger logger, string functorId, T arg, Action<T> action, IActiveObjectContext context, ICustomThreadPool threadPool, ISerializationAnchor serializationAnchor)
            : base(logger, context, threadPool, serializationAnchor)
        {
            _functorId = functorId;

            _action = action;
            _arg = arg;
        }

        private string _functorId;

        //[SocSerializableActionMember(nameof(_functorId), 0)]
        private Action<T> _action;
        private T _arg;

        /// <inheritdoc/>
        protected override void OnRun(CancellationToken cancellationToken)
        {
            _action(_arg);
        }
    }

    public class BaseFunctorWithoutResult<T1, T2> : BaseFunctor
    {
        public BaseFunctorWithoutResult(IMonitorLogger logger, string functorId, T1 arg1, T2 arg2, Action<T1, T2> action, IActiveObjectContext context, ICustomThreadPool threadPool, ISerializationAnchor serializationAnchor)
            : base(logger, context, threadPool, serializationAnchor)
        {
            _functorId = functorId;

            _action = action;
            _arg1 = arg1;
            _arg2 = arg2;
        }

        private string _functorId;

        //[SocSerializableActionMember(nameof(_functorId), 0)]
        private Action<T1, T2> _action;
        private T1 _arg1;
        private T2 _arg2;

        /// <inheritdoc/>
        protected override void OnRun(CancellationToken cancellationToken)
        {
            _action(_arg1, _arg2);
        }
    }

    public class BaseFunctorWithoutResult<T1, T2, T3> : BaseFunctor
    {
        public BaseFunctorWithoutResult(IMonitorLogger logger, string functorId, T1 arg1, T2 arg2, T3 arg3, Action<T1, T2, T3> action, IActiveObjectContext context, ICustomThreadPool threadPool, ISerializationAnchor serializationAnchor)
            : base(logger, context, threadPool, serializationAnchor)
        {
            _functorId = functorId;

            _action = action;
            _arg1 = arg1;
            _arg2 = arg2;
            _arg3 = arg3;
        }

        private string _functorId;

        //[SocSerializableActionMember(nameof(_functorId), 0)]
        private Action<T1, T2, T3> _action;
        private T1 _arg1;
        private T2 _arg2;
        private T3 _arg3;

        /// <inheritdoc/>
        protected override void OnRun(CancellationToken cancellationToken)
        {
            _action(_arg1, _arg2, _arg3);
        }
    }


}
