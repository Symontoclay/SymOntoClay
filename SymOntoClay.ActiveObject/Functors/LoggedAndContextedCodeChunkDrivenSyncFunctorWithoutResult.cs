using SymOntoClay.ActiveObject.CodeChunks;
using SymOntoClay.ActiveObject.CodeChunks.Implementation;
using SymOntoClay.ActiveObject.MethodResponses;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Serialization;
using System;

namespace SymOntoClay.ActiveObject.Functors
{
    public class LoggedAndContextedCodeChunkDrivenSyncFunctorWithoutResult<TGlobalContext, TLocalContext>: IBaseLoggedCodeChunkDrivenSyncFunctorWithoutResult
        where TLocalContext : class, new()
    {
        public static LoggedAndContextedCodeChunkDrivenSyncFunctorWithoutResult<TGlobalContext, TLocalContext> Run(IMonitorLogger logger, string functorId, TGlobalContext globalContext,
            Action<ICodeChunksContext<IMonitorLogger, TGlobalContext, TLocalContext>> action)
        {
            var functor = new LoggedAndContextedCodeChunkDrivenSyncFunctorWithoutResult<TGlobalContext, TLocalContext>(logger, functorId, globalContext, action);
            functor.Run();
            return functor;
        }

        public LoggedAndContextedCodeChunkDrivenSyncFunctorWithoutResult(IMonitorLogger logger, string functorId, TGlobalContext globalContext,
            Action<ICodeChunksContext<IMonitorLogger, TGlobalContext, TLocalContext>> action)
        {
            _localContext = new TLocalContext();

            _functorId = functorId;
            _codeChunksContext = new CodeChunksContext<IMonitorLogger, TGlobalContext, TLocalContext>(logger, globalContext, _localContext);
            _action = action;
            _logger = logger;
            _globalContext = globalContext;
        }

        private string _functorId;

        [SocSerializableActionMember(nameof(_functorId), 0)]
        private Action<ICodeChunksContext<IMonitorLogger, TGlobalContext, TLocalContext>> _action;
        private CodeChunksContext<IMonitorLogger, TGlobalContext, TLocalContext> _codeChunksContext;
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

        void IBaseLoggedCodeChunkDrivenSyncFunctorWithoutResult.ExecuteCodeChunksContext()
        {
            if(_isFinishedCodeChunksContext)
            {
                return;
            }

            _codeChunksContext.Run();

            _isFinishedCodeChunksContext = true;
        }

        bool IBaseLoggedCodeChunkDrivenSyncFunctorWithoutResult.IsFinished => _codeChunksContext.IsFinished;

        public IDrivenSyncMethodResponse ToMethodResponse()
        {
            return new DrivenSyncMethodResponse(this);
        }
    }
}
