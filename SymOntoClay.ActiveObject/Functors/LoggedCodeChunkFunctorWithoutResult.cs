using SymOntoClay.ActiveObject.Threads;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Threading;
using System.Threading;
using System;
using SymOntoClay.ActiveObject.CodeChunks.Implementation;
using SymOntoClay.Serialization;
using SymOntoClay.ActiveObject.CodeChunks;

namespace SymOntoClay.ActiveObject.Functors
{
    public class LoggedCodeChunkFunctorWithoutResult<TGlobalContext, TLocalContext> : BaseFunctor
        where TLocalContext : class, new()
    {
        public static LoggedCodeChunkFunctorWithoutResult<TGlobalContext, TLocalContext> Run(IMonitorLogger logger, string functorId, TGlobalContext globalContext,
            Action<ICodeChunksContext<IMonitorLogger, TGlobalContext, TLocalContext>> action,
            IActiveObjectContext context, ICustomThreadPool threadPool)
        {
            var functor = new LoggedCodeChunkFunctorWithoutResult<TGlobalContext, TLocalContext>(logger, functorId, globalContext,
            action, context, threadPool);
            functor.Run();
            return functor;
        }

        public LoggedCodeChunkFunctorWithoutResult(IMonitorLogger logger, string functorId, TGlobalContext globalContext,
            Action<ICodeChunksContext<IMonitorLogger, TGlobalContext, TLocalContext>> action,
            IActiveObjectContext context, ICustomThreadPool threadPool)
            : base(logger, context, threadPool)
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

        /// <inheritdoc/>
        protected override void OnRun(CancellationToken cancellationToken)
        {
            _action(_codeChunksContext);

            _codeChunksContext.Run();
        }
    }
}
