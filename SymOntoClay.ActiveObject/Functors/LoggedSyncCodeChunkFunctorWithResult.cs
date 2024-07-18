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
            _codeChunksContext = new CodeChunksContext<TResult>(codeChunksContextId, codeChunk);
            _action = action;
            _logger = logger;
            _localContext = new TLocalContext();
            _arg = arg;
        }

        private readonly CodeChunksContext<TResult> _codeChunksContext;
        private readonly IMonitorLogger _logger;
        private readonly Action<CodeChunksContext<TResult>, TLocalContext, T> _action;
        private readonly TLocalContext _localContext;
        private readonly T _arg;
        private TResult _result = default;

        /// <inheritdoc/>
        public override TResult GetResult()
        {
            return _result;
        }

        public void Run()
        {
            _action(_codeChunksContext, _localContext, _arg);

            _codeChunksContext.Run();

            _result = _codeChunksContext.Result;
        }
    }
}
