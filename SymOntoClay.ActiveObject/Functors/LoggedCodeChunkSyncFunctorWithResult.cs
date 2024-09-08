using SymOntoClay.ActiveObject.CodeChunks;
using SymOntoClay.ActiveObject.CodeChunks.Implementation;
using SymOntoClay.ActiveObject.MethodResponses;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Serialization;
using System;

namespace SymOntoClay.ActiveObject.Functors
{
    public class LoggedCodeChunkSyncFunctorWithResult<TGlobalContext, TLocalContext, TResult>: IBaseLoggedCodeChunkSyncFunctorWithResult<TResult>
        where TLocalContext : class, new()
    {
        public static LoggedCodeChunkSyncFunctorWithResult<TGlobalContext, TLocalContext, TResult> Run(IMonitorLogger logger, string functorId, TGlobalContext globalContext,
            Action<ICodeChunksContextWithResult<IMonitorLogger, TGlobalContext, TLocalContext, TResult>> action)
        {
            var functor = new LoggedCodeChunkSyncFunctorWithResult<TGlobalContext, TLocalContext, TResult>(logger, functorId, globalContext, action);
            functor.Run();
            return functor;
        }

        public LoggedCodeChunkSyncFunctorWithResult(IMonitorLogger logger, string functorId, TGlobalContext globalContext,
            Action<ICodeChunksContextWithResult<IMonitorLogger, TGlobalContext, TLocalContext, TResult>> action)
        {
            _localContext = new TLocalContext();

            _functorId = functorId;
            _codeChunksContext = new CodeChunksContextWithResult<IMonitorLogger, TGlobalContext, TLocalContext, TResult>(logger, globalContext, _localContext);
            _action = action;
            _logger = logger;
            _globalContext = globalContext;
        }

        [SocSerializableActionKey]
        private string _functorId;

        private Action<ICodeChunksContextWithResult<IMonitorLogger, TGlobalContext, TLocalContext, TResult>> _action;
        private CodeChunksContextWithResult<IMonitorLogger, TGlobalContext, TLocalContext, TResult> _codeChunksContext;

        private IMonitorLogger _logger;
        private TGlobalContext _globalContext;
        private TLocalContext _localContext;

        public void Run()
        {
            _action(_codeChunksContext);
        }

        void IBaseLoggedCodeChunkSyncFunctorWithResult<TResult>.ExecuteCodeChunksContext()
        {
            _codeChunksContext.Run();
        }

        bool IBaseLoggedCodeChunkSyncFunctorWithResult<TResult>.IsFinished => _codeChunksContext.IsFinished;

        TResult IBaseLoggedCodeChunkSyncFunctorWithResult<TResult>.Result => _codeChunksContext.Result;

        public ISyncMethodResponse<TResult> ToMethodResponse()
        {
            return new SyncMethodResponse<TResult>(this);
        }
    }
}
