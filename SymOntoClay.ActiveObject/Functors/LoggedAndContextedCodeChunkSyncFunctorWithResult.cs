using SymOntoClay.ActiveObject.CodeChunks;
using SymOntoClay.ActiveObject.CodeChunks.Implementation;
using SymOntoClay.ActiveObject.MethodResponses;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Serialization;
using System;

namespace SymOntoClay.ActiveObject.Functors
{
    public class LoggedAndContextedCodeChunkSyncFunctorWithResult<TGlobalContext, TLocalContext, TResult> : IBaseFunctor
        where TLocalContext : class, new()
    {
        public static LoggedAndContextedCodeChunkSyncFunctorWithResult<TGlobalContext, TLocalContext, TResult> Run(IMonitorLogger logger, string functorId, TGlobalContext globalContext,
            Action<ICodeChunksContextWithResult<IMonitorLogger, TGlobalContext, TLocalContext, TResult>> action,
            ISerializationAnchor serializationAnchor)
        {
            var functor = new LoggedAndContextedCodeChunkSyncFunctorWithResult<TGlobalContext, TLocalContext, TResult>(logger, functorId, globalContext,
            action, serializationAnchor);
            functor.Run();
            return functor;
        }

        public LoggedAndContextedCodeChunkSyncFunctorWithResult(IMonitorLogger logger, string functorId, TGlobalContext globalContext,
            Action<ICodeChunksContextWithResult<IMonitorLogger, TGlobalContext, TLocalContext, TResult>> action,
            ISerializationAnchor serializationAnchor)
        {
            _serializationAnchor = serializationAnchor;
            serializationAnchor.AddFunctor(this);

            _localContext = new TLocalContext();

            _functorId = functorId;
            _codeChunksContext = new CodeChunksContextWithResult<IMonitorLogger, TGlobalContext, TLocalContext, TResult>(logger, globalContext, _localContext);
            _action = action;
            _logger = logger;
            _globalContext = globalContext;
        }

        [SocSerializableActionKey]
        private string _functorId;

        private ISerializationAnchor _serializationAnchor;

        private Action<ICodeChunksContextWithResult<IMonitorLogger, TGlobalContext, TLocalContext, TResult>> _action;
        private CodeChunksContextWithResult<IMonitorLogger, TGlobalContext, TLocalContext, TResult> _codeChunksContext;

        private IMonitorLogger _logger;
        private TGlobalContext _globalContext;
        private TLocalContext _localContext;

        private bool _isFinished;
        private bool _isFinishedRun;
        private bool _isFinishedCodeChunksContext;

        public void Run()
        {
            if (_isFinished)
            {
                return;
            }

            if(!_isFinishedRun)
            {
                _action(_codeChunksContext);

                _isFinishedRun = true;
            }

            if(!_isFinishedCodeChunksContext)
            {
                _codeChunksContext.Run();

                _isFinishedCodeChunksContext = true;
            }

            _isFinished = true;

            _serializationAnchor.RemoveFunctor(this);
        }

        public IAsyncMethodResponse<TResult> ToMethodResponse()
        {
            return new CompletedAsyncMethodResponse<TResult>(_codeChunksContext.Result);
        }
    }
}
