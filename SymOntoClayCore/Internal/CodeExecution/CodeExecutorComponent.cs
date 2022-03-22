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
        public Value ExecuteBatchAsync(List<ProcessInitialInfo> processInitialInfoList)
        {
            return ExecuteBatchAsync(processInitialInfoList, null);
        }

        /// <inheritdoc/>
        public Value ExecuteAsync(ProcessInitialInfo processInitialInfo)
        {
            return ExecuteAsync(processInitialInfo, null);
        }

        /// <inheritdoc/>
        public Value ExecuteBatchAsync(List<ProcessInitialInfo> processInitialInfoList, IExecutionCoordinator actionExecutionCoordinator)
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

                var processInfo = new ProcessInfo();

                codeFrame.ProcessInfo = processInfo;
                processInfo.CodeFrame = codeFrame;
                codeFrame.Metadata = processInitialInfo.Metadata;

                _context.InstancesStorage.AppendProcessInfo(processInfo);

                codeFramesList.Add(codeFrame);
            }

#if DEBUG
            //_context.InstancesStorage.PrintProcessesList();
#endif

            var threadExecutor = new AsyncThreadExecutor(_context, actionExecutionCoordinator);
            threadExecutor.SetCodeFrames(codeFramesList);

            return threadExecutor.Start();
        }

        /// <inheritdoc/>
        public Value ExecuteAsync(ProcessInitialInfo processInitialInfo, IExecutionCoordinator actionExecutionCoordinator)
        {
            return ExecuteBatchAsync(new List<ProcessInitialInfo>() { processInitialInfo }, actionExecutionCoordinator);
        }
    }
}
