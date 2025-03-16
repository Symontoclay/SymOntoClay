using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Monitor.Common;
using System.Collections.Generic;

namespace SymOntoClay.Core.Internal.Converters
{
    public interface ITypeConverter
    {
        CallResult CheckAndTryConvert(IMonitorLogger logger, Value value, IList<StrongIdentifierValue> typesList, ILocalCodeExecutionContext localCodeExecutionContext);
        TypeFitCheckingResult CheckFitValue(IMonitorLogger logger, Value value, IList<StrongIdentifierValue> typesList, ILocalCodeExecutionContext localCodeExecutionContext);
        Value Convert(IMonitorLogger logger, Value value, StrongIdentifierValue targetType, ILocalCodeExecutionContext localCodeExecutionContext);
    }
}
