using NLog;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.IndexedData.ScriptingData;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace SymOntoClay.Core.Internal.CodeModel.Handlers
{
    public class CancellationTokenSourceCancelHandler: BaseSpecialHandler
    {
#if DEBUG
        //private static ILogger _gbcLogger = LogManager.GetCurrentClassLogger();
#endif

        public CancellationTokenSourceCancelHandler(CancellationTokenSource cancellationTokenSource)
        {
            _cancellationTokenSource = cancellationTokenSource;
        }

        private readonly CancellationTokenSource _cancellationTokenSource;

        /// <inheritdoc/>
        public override Value Call(IList<Value> paramsList, LocalCodeExecutionContext localCodeExecutionContext)
        {
#if DEBUG
            //_gbcLogger.Info($"paramsList?.Count = {paramsList?.Count}");
#endif

            return NCall();
        }

        /// <inheritdoc/>
        public override Value Call(IDictionary<string, Value> paramsDict, Value anotation, LocalCodeExecutionContext localCodeExecutionContext)
        {
#if DEBUG
            //_gbcLogger.Info($"paramsDict?.Count = {paramsDict?.Count}");
#endif

            return NCall();
        }

        private Value NCall()
        {
            _cancellationTokenSource.Cancel();

            return NullValue.Instance;
        }
    }
}
