using SymOntoClay.Core.Internal.Threads;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Threading;
using System;
using System.Threading;

namespace SymOntoClay.Core.Internal.Serialization.Functors
{
    public class BaseFunctorWithResult<TResult> : BaseFunctor<TResult>
    {
        public BaseFunctorWithResult(IMonitorLogger logger, Func<TResult> func, IActiveObjectContext context, ICustomThreadPool threadPool)
            : base(logger, context, threadPool)
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

    public class BaseFunctorWithResult<T, TResult> : BaseFunctor
    {
        public BaseFunctorWithResult(IMonitorLogger logger, T arg, Func<T, TResult> func, IActiveObjectContext context, ICustomThreadPool threadPool)
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
        public BaseFunctorWithResult(IMonitorLogger logger, T1 arg1, T2 arg2, Func<T1, T2, TResult> func, IActiveObjectContext context, ICustomThreadPool threadPool)
            : base(logger, context, threadPool)
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
}
