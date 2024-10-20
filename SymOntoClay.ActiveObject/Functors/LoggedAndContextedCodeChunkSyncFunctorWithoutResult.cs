using SymOntoClay.ActiveObject.CodeChunks;
using SymOntoClay.ActiveObject.CodeChunks.Implementation;
using SymOntoClay.ActiveObject.MethodResponses;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Serialization;
using System;

namespace SymOntoClay.ActiveObject.Functors
{
    public class LoggedAndContextedCodeChunkSyncFunctorWithoutResult<TGlobalContext, TLocalContext> : IBaseFunctor
        where TLocalContext : class, new()
    {
        public static LoggedAndContextedCodeChunkSyncFunctorWithoutResult<TGlobalContext, TLocalContext> Run(IMonitorLogger logger, string functorId, TGlobalContext globalContext,
            Action<ICodeChunksContext<IMonitorLogger, TGlobalContext, TLocalContext>> action,
            ISerializationAnchor serializationAnchor)
        {
            var functor = new LoggedAndContextedCodeChunkSyncFunctorWithoutResult<TGlobalContext, TLocalContext>(logger, functorId, globalContext,
            action, serializationAnchor);
            functor.Run();
            return functor;
        }

        public LoggedAndContextedCodeChunkSyncFunctorWithoutResult(IMonitorLogger logger, string functorId, TGlobalContext globalContext,
            Action<ICodeChunksContext<IMonitorLogger, TGlobalContext, TLocalContext>> action,
            ISerializationAnchor serializationAnchor)
        {
            _serializationAnchor = serializationAnchor;
            serializationAnchor.AddFunctor(this);

            _localContext = new TLocalContext();

            _functorId = functorId;
            _codeChunksContext = new CodeChunksContext<IMonitorLogger, TGlobalContext, TLocalContext>(logger, globalContext, _localContext);
            _action = action;
            _logger = logger;
            _globalContext = globalContext;
        }

        private string _functorId;

        private ISerializationAnchor _serializationAnchor;

        [SocSerializableActionMember(nameof(_functorId), 0)]
        private Action<ICodeChunksContext<IMonitorLogger, TGlobalContext, TLocalContext>> _action;
        private CodeChunksContext<IMonitorLogger, TGlobalContext, TLocalContext> _codeChunksContext;
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

            if (!_isFinishedRun)
            {
                _action(_codeChunksContext);

                _isFinishedRun = true;
            }

            if (!_isFinishedCodeChunksContext)
            {
                _codeChunksContext.Run();

                _isFinishedCodeChunksContext = true;
            }

            _isFinished = true;

            _serializationAnchor.RemoveFunctor(this);
        }

        public IAsyncMethodResponse ToMethodResponse()
        {
            return CompletedAsyncMethodResponse.Instance;
        }
    }
}
