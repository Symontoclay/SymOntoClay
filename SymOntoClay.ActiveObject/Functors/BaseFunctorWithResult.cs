using SymOntoClay.ActiveObject.Threads;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Threading;
using System;
using System.Threading;

namespace SymOntoClay.ActiveObject.Functors
{
    public class BaseFunctorWithResult<TResult> : BaseFunctor<TResult>
    {
        public BaseFunctorWithResult(IMonitorLogger logger, Func<TResult> func, IActiveObjectContext context, ICustomThreadPool threadPool, ISerializationAnchor serializationAnchor)
            : base(logger, context, threadPool, serializationAnchor)
        {
            _func = func;
        }

        private readonly Func<TResult> _func;

        /// <inheritdoc/>
        protected override TResult OnRun(CancellationToken cancellationToken)
        {
            return _func();
        }
    }

    public class BaseFunctorWithResult<T, TResult> : BaseFunctor<TResult>
    {
        public BaseFunctorWithResult(IMonitorLogger logger, T arg, Func<T, TResult> func, IActiveObjectContext context, ICustomThreadPool threadPool, ISerializationAnchor serializationAnchor)
            : base(logger, context, threadPool, serializationAnchor)
        {
            _func = func;
            _arg = arg;
        }

        private readonly Func<T, TResult> _func;
        private readonly T _arg;

        /// <inheritdoc/>
        protected override TResult OnRun(CancellationToken cancellationToken)
        {
            return _func(_arg);
        }
    }

    public class BaseFunctorWithResult<T1, T2, TResult> : BaseFunctor<TResult>
    {
        public BaseFunctorWithResult(IMonitorLogger logger, T1 arg1, T2 arg2, Func<T1, T2, TResult> func, IActiveObjectContext context, ICustomThreadPool threadPool, ISerializationAnchor serializationAnchor)
            : base(logger, context, threadPool, serializationAnchor)
        {
            _func = func;
            _arg1 = arg1;
            _arg2 = arg2;
        }

        private readonly Func<T1, T2, TResult> _func;
        private readonly T1 _arg1;
        private readonly T2 _arg2;

        /// <inheritdoc/>
        protected override TResult OnRun(CancellationToken cancellationToken)
        {
            return _func(_arg1, _arg2);
        }
    }

    public class BaseFunctorWithResult<T1, T2, T3, TResult> : BaseFunctor<TResult>
    {
        public BaseFunctorWithResult(IMonitorLogger logger, T1 arg1, T2 arg2, T3 arg3, Func<T1, T2, T3, TResult> func, IActiveObjectContext context, ICustomThreadPool threadPool, ISerializationAnchor serializationAnchor)
            : base(logger, context, threadPool, serializationAnchor)
        {
            _func = func;
            _arg1 = arg1;
            _arg2 = arg2;
            _arg3 = arg3;
        }

        private readonly Func<T1, T2, T3, TResult> _func;
        private readonly T1 _arg1;
        private readonly T2 _arg2;
        private readonly T3 _arg3;

        /// <inheritdoc/>
        protected override TResult OnRun(CancellationToken cancellationToken)
        {
            return _func(_arg1, _arg2, _arg3);
        }
    }
}
