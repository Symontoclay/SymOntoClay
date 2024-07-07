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
            var result = _func();

            DisposeActiveObject();

            return result;
        }
    }

    //public class BaseFunctorWithResult<T, TResult> : BaseFunctor
    //{
    //    public BaseFunctorWithResult()
    //    {
    //    }
    //}

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
            throw new NotImplementedException("1BF15EB4-184D-4D05-A1D7-CEBC2F54FD2A");
        }
    }
}
