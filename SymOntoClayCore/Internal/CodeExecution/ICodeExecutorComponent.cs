/*MIT License

Copyright (c) 2020 - 2024 Sergiy Tolkachov

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.*/

using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Ast.Expressions;
using SymOntoClay.Monitor.Common;
using System.Collections.Generic;

namespace SymOntoClay.Core.Internal.CodeExecution
{
    public interface ICodeExecutorComponent
    {
        IThreadExecutor ExecuteBatchAsync(IMonitorLogger logger, List<ProcessInitialInfo> processInitialInfoList);
        IThreadExecutor ExecuteBatchAsync(IMonitorLogger logger, List<ProcessInitialInfo> processInitialInfoList, string parentThreadLoggerId);
        IThreadExecutor ExecuteBatchSync(IMonitorLogger logger, List<ProcessInitialInfo> processInitialInfoList);
        IThreadExecutor ExecuteAsync(IMonitorLogger logger, ProcessInitialInfo processInitialInfo);
        IThreadExecutor ExecuteAsync(IMonitorLogger logger, ProcessInitialInfo processInitialInfo, string parentThreadLoggerId);
        Value CallOperator(IMonitorLogger logger, KindOfOperator kindOfOperator, List<Value> paramsList, ILocalCodeExecutionContext parentLocalCodeExecutionContext, CallMode callMode);
        Value CallExecutableSync(IMonitorLogger logger, IExecutable executable, List<Value> positionedParameters, ILocalCodeExecutionContext parentLocalCodeExecutionContext, CallMode callMode);
        Value CallFunctionSync(IMonitorLogger logger, Value caller, KindOfFunctionParameters kindOfParameters, List<Value> parameters, ILocalCodeExecutionContext parentLocalCodeExecutionContext);
    }
}
