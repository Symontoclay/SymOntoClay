using SymOntoClay.ActiveObject.CodeChunks;
using SymOntoClay.ActiveObject.CodeChunks.Implementation;
using SymOntoClay.ActiveObject.Threads;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Serialization;
using SymOntoClay.Threading;
using System;
using System.Threading;

namespace SymOntoClay.ActiveObject.Functors
{
    public partial class LoggedAndContextedCodeChunkFunctorWithResult<TGlobalContext, TLocalContext, TResult> : BaseFunctor<TResult>
        where TLocalContext : class, new()
    {
        public static LoggedAndContextedCodeChunkFunctorWithResult<TGlobalContext, TLocalContext, TResult> Run(IMonitorLogger logger, string functorId, TGlobalContext globalContext,
            Action<ICodeChunksContextWithResult<IMonitorLogger, TGlobalContext, TLocalContext, TResult>> action,
            IActiveObjectContext context, ICustomThreadPool threadPool, ISerializationAnchor serializationAnchor)
        {
            var functor = new LoggedAndContextedCodeChunkFunctorWithResult<TGlobalContext, TLocalContext, TResult>(logger, functorId, globalContext,
            action, context, threadPool, serializationAnchor);
            functor.Run();
            return functor;
        }

        public LoggedAndContextedCodeChunkFunctorWithResult(IMonitorLogger logger, string functorId, TGlobalContext globalContext,
            Action<ICodeChunksContextWithResult<IMonitorLogger, TGlobalContext, TLocalContext, TResult>> action,
            IActiveObjectContext context, ICustomThreadPool threadPool, ISerializationAnchor serializationAnchor)
            : base(logger, context, threadPool, serializationAnchor)
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

        /// <inheritdoc/>
        protected override TResult OnRun(CancellationToken cancellationToken)
        {
            _action(_codeChunksContext);

            _codeChunksContext.Run();

            return _codeChunksContext.Result;
        }
    }
}
