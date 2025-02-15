using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Monitor.Common;
using System.Collections.Generic;
using System.Threading;

namespace SymOntoClay.Core.Internal.CodeModel.Handlers
{
    public class ThreadExecutorCancelHandler : BaseSpecialHandler
    {
        public ThreadExecutorCancelHandler(IThreadExecutor threadExecutor)
        { 
            _threadExecutor = threadExecutor;
        }

        private readonly IThreadExecutor _threadExecutor;

        /// <inheritdoc/>
        public override CallResult Call(IMonitorLogger logger, IList<Value> paramsList, ILocalCodeExecutionContext localCodeExecutionContext, CallMode callMode)
        {
            return NCall(logger);
        }

        /// <inheritdoc/>
        public override CallResult Call(IMonitorLogger logger, IDictionary<string, Value> paramsDict, Value annotation, ILocalCodeExecutionContext localCodeExecutionContext, CallMode callMode)
        {
            return NCall(logger);
        }

        private CallResult NCall(IMonitorLogger logger)
        {
            _threadExecutor.Cancel();

            return new CallResult(NullValue.Instance);
        }
    }
}
