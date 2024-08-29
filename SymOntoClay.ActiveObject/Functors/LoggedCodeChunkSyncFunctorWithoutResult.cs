using SymOntoClay.ActiveObject.CodeChunks;
using SymOntoClay.ActiveObject.CodeChunks.Implementation;
using SymOntoClay.ActiveObject.MethodResponses;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Serialization;
using System;

namespace SymOntoClay.ActiveObject.Functors
{
    public class LoggedCodeChunkSyncFunctorWithoutResult<TGlobalContext, TLocalContext>: IBaseLoggedCodeChunkSyncFunctorWithoutResult
        where TLocalContext : class, new()
    {
        public static LoggedCodeChunkSyncFunctorWithoutResult<TGlobalContext, TLocalContext> Run(IMonitorLogger logger, string functorId, TGlobalContext globalContext,
            Action<ICodeChunksContext<IMonitorLogger, TGlobalContext, TLocalContext>> action)
        {
            var functor = new LoggedCodeChunkSyncFunctorWithoutResult<TGlobalContext, TLocalContext>(logger, functorId, globalContext, action);
            functor.Run();
            return functor;
        }

        public LoggedCodeChunkSyncFunctorWithoutResult(IMonitorLogger logger, string functorId, TGlobalContext globalContext,
            Action<ICodeChunksContext<IMonitorLogger, TGlobalContext, TLocalContext>> action)
        {
            _localContext = new TLocalContext();

            _functorId = functorId;
            _codeChunksContext = new CodeChunksContext<IMonitorLogger, TGlobalContext, TLocalContext>(logger, globalContext, _localContext);
            _action = action;
            _logger = logger;
            _globalContext = globalContext;
        }

        [SocSerializableActionKey]
        private string _functorId;

        private Action<ICodeChunksContext<IMonitorLogger, TGlobalContext, TLocalContext>> _action;
        private CodeChunksContext<IMonitorLogger, TGlobalContext, TLocalContext> _codeChunksContext;
        private IMonitorLogger _logger;
        private TGlobalContext _globalContext;
        private TLocalContext _localContext;

        public void Run()
        {
            _action(_codeChunksContext);
        }

        void IBaseLoggedCodeChunkSyncFunctorWithoutResult.ExecuteCodeChunksContext()
        {
            _codeChunksContext.Run();
        }

        bool IBaseLoggedCodeChunkSyncFunctorWithoutResult.IsFinished => _codeChunksContext.IsFinished;

        public ISyncMethodResponse ToMethodResponse()
        {
            return new SyncMethodResponse(this);
        }
    }
}
