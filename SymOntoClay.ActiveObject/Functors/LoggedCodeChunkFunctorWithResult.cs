using SymOntoClay.ActiveObject.Threads;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Threading;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

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
            throw new NotImplementedException("DCCE2875-99F5-4CED-9CB7-A83CE18EC7AE");
        }

        public LoggedCodeChunkFunctorWithResult(IMonitorLogger logger, string codeChunksContextId, TGlobalContext globalContext, T arg,
            Action<IMonitorLogger, CodeChunksContext<TResult>, TGlobalContext, TLocalContext, T> action,
            IActiveObjectContext context, ICustomThreadPool threadPool)
            : base(logger, context, threadPool)
        {
        }

        /// <inheritdoc/>
        protected override TResult OnRun(CancellationToken cancellationToken)
        {
            throw new NotImplementedException("4FA7D156-5DA0-45C8-9D0B-CD20BE882F28");
        }
    }
}
