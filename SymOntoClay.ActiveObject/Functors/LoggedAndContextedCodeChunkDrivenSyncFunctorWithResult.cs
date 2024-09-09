using SymOntoClay.ActiveObject.CodeChunks;
using SymOntoClay.ActiveObject.CodeChunks.Implementation;
using SymOntoClay.ActiveObject.MethodResponses;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Serialization;
using System;

namespace SymOntoClay.ActiveObject.Functors
{
    public class LoggedAndContextedCodeChunkDrivenSyncFunctorWithResult<TGlobalContext, TLocalContext, TResult>: IBaseLoggedCodeChunkDrivenSyncFunctorWithResult<TResult>
        where TLocalContext : class, new()
    {
        public static LoggedAndContextedCodeChunkDrivenSyncFunctorWithResult<TGlobalContext, TLocalContext, TResult> Run(IMonitorLogger logger, string functorId, TGlobalContext globalContext,
            Action<ICodeChunksContextWithResult<IMonitorLogger, TGlobalContext, TLocalContext, TResult>> action)
        {
            var functor = new LoggedAndContextedCodeChunkDrivenSyncFunctorWithResult<TGlobalContext, TLocalContext, TResult>(logger, functorId, globalContext, action);
            functor.Run();
            return functor;
        }

        public LoggedAndContextedCodeChunkDrivenSyncFunctorWithResult(IMonitorLogger logger, string functorId, TGlobalContext globalContext,
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

        private bool _isFinishedRun;
        private bool _isFinishedCodeChunksContext;

        public void Run()
        {
            if (_isFinishedRun)
            {
                return;
            }

            _action(_codeChunksContext);

            _isFinishedRun = true;
        }

        void IBaseLoggedCodeChunkDrivenSyncFunctorWithResult<TResult>.ExecuteCodeChunksContext()
        {
            if (_isFinishedCodeChunksContext)
            {
                return;
            }

            _codeChunksContext.Run();

            _isFinishedCodeChunksContext = true;
        }

        bool IBaseLoggedCodeChunkDrivenSyncFunctorWithResult<TResult>.IsFinished => _codeChunksContext.IsFinished;

        TResult IBaseLoggedCodeChunkDrivenSyncFunctorWithResult<TResult>.Result => _codeChunksContext.Result;

        public ISyncMethodResponse<TResult> ToMethodResponse()
        {
            return new SyncMethodResponse<TResult>(this);
        }
    }
}
