using SymOntoClay.ActiveObject.Threads;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Threading;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using static System.Collections.Specialized.BitVector32;

namespace SymOntoClay.ActiveObject.Functors
{
    //public class LoggedCodeChunkFunctorWithResult<TGlobalContext, TLocalContext, TResult> : BaseFunctor<TResult>
    //    where TLocalContext : class, new()
    //{
    //    public LoggedCodeChunkFunctorWithResult()
    //    {
    //    }
    //}

    public class LoggedCodeChunkFunctorWithResult<TGlobalContext, TLocalContext, T, TResult> : BaseFunctor<TResult>
        where TLocalContext : class, new()
    {
        public static LoggedCodeChunkFunctorWithResult<TGlobalContext, TLocalContext, T, TResult> Run(IMonitorLogger logger, string codeChunksContextId, TGlobalContext globalContext, T arg,
            Action<IMonitorLogger, CodeChunksContext<TResult>, TGlobalContext, TLocalContext, T> action,
            IActiveObjectContext context, ICustomThreadPool threadPool)
        {
            var functor = new LoggedCodeChunkFunctorWithResult<TGlobalContext, TLocalContext, T, TResult>(logger, codeChunksContextId, globalContext, arg,
                action, context, threadPool);
            functor.Run();
            return functor;
        }

        public LoggedCodeChunkFunctorWithResult(IMonitorLogger logger, string codeChunksContextId, TGlobalContext globalContext, T arg,
            Action<IMonitorLogger, CodeChunksContext<TResult>, TGlobalContext, TLocalContext, T> action,
            IActiveObjectContext context, ICustomThreadPool threadPool)
            : base(logger, context, threadPool)
        {
            _codeChunksContext = new CodeChunksContext<TResult>(codeChunksContextId);
            _action = action;
            _logger = logger;
            _globalContext = globalContext;
            _localContext = new TLocalContext();
            _arg = arg;
        }

        private readonly Action<IMonitorLogger, CodeChunksContext<TResult>, TGlobalContext, TLocalContext, T> _action;
        private readonly CodeChunksContext<TResult> _codeChunksContext;
        private readonly IMonitorLogger _logger;
        private readonly TGlobalContext _globalContext;
        private readonly TLocalContext _localContext;
        private readonly T _arg;

        /// <inheritdoc/>
        protected override TResult OnRun(CancellationToken cancellationToken)
        {
            _action(_logger, _codeChunksContext, _globalContext, _localContext, _arg);

            _codeChunksContext.Run();

            return _codeChunksContext.Result;
        }
    }
}
