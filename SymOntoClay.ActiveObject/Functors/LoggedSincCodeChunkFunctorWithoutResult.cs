﻿using SymOntoClay.ActiveObject.MethodResponses;
using SymOntoClay.Monitor.Common;
using System;
using System.Collections.Generic;
using System.Text;
using static System.Collections.Specialized.BitVector32;

namespace SymOntoClay.ActiveObject.Functors
{
    public class LoggedSincCodeChunkFunctorWithoutResult<TLocalContext>: BaseLoggedSincCodeChunkFunctor
        where TLocalContext : class, new()
    {
        public static LoggedSincCodeChunkFunctorWithoutResult<TLocalContext> Run(IMonitorLogger logger, string codeChunksContextId, ICodeChunk codeChunk, Action<CodeChunksContext, TLocalContext> action)
        {
            var functor = new LoggedSincCodeChunkFunctorWithoutResult<TLocalContext>(logger, codeChunksContextId, codeChunk, action);
            functor.Run();
            return functor;
        }

        public LoggedSincCodeChunkFunctorWithoutResult(IMonitorLogger logger, string codeChunksContextId, ICodeChunk codeChunk, Action<CodeChunksContext, TLocalContext> action)
        {
            _codeChunksContext = new CodeChunksContext(codeChunksContextId, codeChunk);
            _action = action;
            _logger = logger;
            _localContext = new TLocalContext();
        }

        private readonly CodeChunksContext _codeChunksContext;
        private readonly IMonitorLogger _logger;
        private readonly Action<CodeChunksContext, TLocalContext> _action;
        private readonly TLocalContext _localContext;

        public void Run()
        {
            _action(_codeChunksContext, _localContext);

            _codeChunksContext.Run();
        }
    }
}
