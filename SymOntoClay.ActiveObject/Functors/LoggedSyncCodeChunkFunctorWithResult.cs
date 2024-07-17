using SymOntoClay.Monitor.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.ActiveObject.Functors
{
    public class LoggedSyncCodeChunkFunctorWithResult<TLocalContext, T, TResult> : BaseLoggedSyncCodeChunkFunctor<TResult>
        where TLocalContext : class, new()
    {
        public static LoggedSyncCodeChunkFunctorWithResult<TLocalContext, T, TResult> Run(IMonitorLogger logger, string codeChunksContextId, ICodeChunk codeChunk, T arg, Action<CodeChunksContext<TResult>, TLocalContext, T> action)
        {
            var functor = new LoggedSyncCodeChunkFunctorWithResult<TLocalContext, T, TResult>(logger, codeChunksContextId, codeChunk, arg, action);
            functor.Run();
            return functor;
        }

        public LoggedSyncCodeChunkFunctorWithResult(IMonitorLogger logger, string codeChunksContextId, ICodeChunk codeChunk, T arg, Action<CodeChunksContext<TResult>, TLocalContext, T> action)
        {
            throw new NotImplementedException("8D0E6990-60C9-4353-9324-13E446E7B0AC");
        }

        public void Run()
        {
            throw new NotImplementedException("4B6573DF-B785-40C1-A779-999AD1E9C515");
        }
    }
}
