/*MIT License

Copyright (c) 2020 - <curr_year/> Sergiy Tolkachov

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

using SymOntoClay.Core.Internal.CodeExecution.Helpers;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.Instances;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeExecution
{
    public class CodeExecutorComponent: BaseComponent, ICodeExecutorComponent
    {
        public CodeExecutorComponent(IEngineContext context)
            : base(context.Logger)
        {
            _context = context;
        }

        private readonly IEngineContext _context;

        /// <inheritdoc/>
        public Value ExecuteAsync(ProcessInitialInfo processInitialInfo)
        {
            return ExecuteBatchAsync(new List<ProcessInitialInfo>() { processInitialInfo });
        }

        /// <inheritdoc/>
        public Value ExecuteBatchAsync(List<ProcessInitialInfo> processInitialInfoList)
        {
#if DEBUG
            //Log($"processInitialInfoList = {processInitialInfoList.WriteListToString()}");
#endif

            var codeFramesList = new List<CodeFrame>();

            foreach (var processInitialInfo in processInitialInfoList)
            {
                var codeFrame = new CodeFrame();
                codeFrame.CompiledFunctionBody = processInitialInfo.CompiledFunctionBody;
                codeFrame.LocalContext = processInitialInfo.LocalContext;

                codeFrame.Instance = processInitialInfo.Instance;
                codeFrame.ExecutionCoordinator = processInitialInfo.ExecutionCoordinator;

                var processInfo = new ProcessInfo();

                codeFrame.ProcessInfo = processInfo;
                processInfo.CodeFrame = codeFrame;
                codeFrame.Metadata = processInitialInfo.Metadata;

                _context.InstancesStorage.AppendProcessInfo(processInfo);

                codeFramesList.Add(codeFrame);
            }

#if DEBUG
            //_context.InstancesStorage.PrintProcessesList();
            //Log("L>>>");
#endif

            var threadExecutor = new AsyncThreadExecutor(_context);
            threadExecutor.SetCodeFrames(codeFramesList);

            return threadExecutor.Start();
        }

        /// <inheritdoc/>
        public Value CallExecutableSync(IExecutable executable, List<Value> positionedParameters, LocalCodeExecutionContext parentLocalCodeExecutionContext)
        {
            return CallExecutable(executable, KindOfFunctionParameters.PositionedParameters, null, positionedParameters, true, parentLocalCodeExecutionContext);
        }

        private Value CallExecutable(IExecutable executable, KindOfFunctionParameters kindOfParameters, Dictionary<StrongIdentifierValue, Value> namedParameters, List<Value> positionedParameters, bool isSync, LocalCodeExecutionContext parentLocalCodeExecutionContext)
        {
            if (executable == null)
            {
                throw new ArgumentNullException(nameof(executable));
            }

            var coordinator = executable.TryActivate(_context);

#if DEBUG
            //Log($"executable.IsSystemDefined = {executable.IsSystemDefined}");
            //Log($"coordinator != null = {coordinator != null}");
            //Log($"isSync = {isSync}");
#endif

            if (executable.IsSystemDefined)
            {
                Value result = null;

                switch (kindOfParameters)
                {
                    case KindOfFunctionParameters.PositionedParameters:
                        result = executable.SystemHandler.Call(positionedParameters, parentLocalCodeExecutionContext);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(kindOfParameters), kindOfParameters, null);
                }

#if DEBUG
                //Log($"result = {result}");
#endif

                return result;
            }
            else
            {
                var newCodeFrame = CodeFrameHelper.ConvertExecutableToCodeFrame(executable, kindOfParameters, namedParameters, positionedParameters, parentLocalCodeExecutionContext, _context);

#if DEBUG
                //Log($"newCodeFrame = {newCodeFrame}");
#endif

                _context.InstancesStorage.AppendProcessInfo(newCodeFrame.ProcessInfo);

#if DEBUG
                //Log($"isSync = {isSync}");
#endif

                if (isSync)
                {
                    newCodeFrame.ExecutionCoordinator = coordinator;

                    //SetCodeFrame(newCodeFrame);

                    throw new NotImplementedException();
                }
                else
                {
                    var threadExecutor = new AsyncThreadExecutor(_context);
                    threadExecutor.SetCodeFrame(newCodeFrame);

                    var task = threadExecutor.Start();

                    return task;
                }
            }
        }
    }
}
