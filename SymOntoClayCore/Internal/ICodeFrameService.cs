using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeExecution.Helpers;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.IndexedData.ScriptingData;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal
{
    public interface ICodeFrameService
    {
        CodeFrame ConvertCompiledFunctionBodyToCodeFrame(CompiledFunctionBody compiledFunctionBody, LocalCodeExecutionContext parentLocalCodeExecutionContext);
        CodeFrame ConvertExecutableToCodeFrame(IExecutable function, KindOfFunctionParameters kindOfParameters,
            Dictionary<StrongIdentifierValue, Value> namedParameters, List<Value> positionedParameters,
            LocalCodeExecutionContext parentLocalCodeExecutionContext, ConversionExecutableToCodeFrameAdditionalSettings additionalSettings = null);
    }
}
