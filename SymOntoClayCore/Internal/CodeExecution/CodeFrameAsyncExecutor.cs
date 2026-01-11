/*MIT License

Copyright (c) 2020 - 2026 Sergiy Tolkachov

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

using NLog;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.Services;
using SymOntoClay.Monitor.Common;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeExecution
{
    public class CodeFrameAsyncExecutor : BaseLoggedComponent
    {
        public CodeFrameAsyncExecutor(IEngineContext context)
            : base(context.Logger)
        {
            _context = context;
            _codeFrameService = context.ServicesFactory.GetCodeFrameService();
        }

        private readonly IEngineContext _context;
        private readonly ICodeFrameService _codeFrameService;

        // <inheritdoc/>
        //public void AsyncExecuteCodeFrame(CodeFrame codeFrame, IExecutionCoordinator coordinator, SyncOption syncOption, bool increaseCurrentFramePosition = true, AnnotationSystemEvent completeAnnotationSystemEvent = null, AnnotationSystemEvent cancelAnnotationSystemEvent = null, AnnotationSystemEvent weakCancelAnnotationSystemEvent = null, AnnotationSystemEvent errorAnnotationSystemEvent = null)
        //{
        //    AsyncExecuteCodeFrame(codeFrame, null, coordinator, syncOption, increaseCurrentFramePosition, completeAnnotationSystemEvent, cancelAnnotationSystemEvent, weakCancelAnnotationSystemEvent, errorAnnotationSystemEvent);
        //}

        // <inheritdoc/>
        //public void AsyncExecuteCodeFrame(CodeFrame codeFrame, CodeFrame currentCodeFrame, IExecutionCoordinator coordinator, SyncOption syncOption, bool increaseCurrentFramePosition = true, AnnotationSystemEvent completeAnnotationSystemEvent = null, AnnotationSystemEvent cancelAnnotationSystemEvent = null, AnnotationSystemEvent weakCancelAnnotationSystemEvent = null, AnnotationSystemEvent errorAnnotationSystemEvent = null)
        //{
        //    AsyncExecuteCodeFrame(string.Empty, codeFrame, currentCodeFrame, coordinator, syncOption, increaseCurrentFramePosition, completeAnnotationSystemEvent, cancelAnnotationSystemEvent, weakCancelAnnotationSystemEvent, errorAnnotationSystemEvent);
        //}

        public void AsyncExecuteCodeFrame(IMonitorLogger logger, string parentThreadId, CodeFrame codeFrame, CodeFrame currentCodeFrame, IExecutionCoordinator coordinator, SyncOption syncOption, bool increaseCurrentFramePosition = true, AnnotationSystemEvent completeAnnotationSystemEvent = null, AnnotationSystemEvent cancelAnnotationSystemEvent = null, AnnotationSystemEvent weakCancelAnnotationSystemEvent = null, AnnotationSystemEvent errorAnnotationSystemEvent = null)
        {
            switch(syncOption)
            {
                case SyncOption.IndependentAsync:
                case SyncOption.ChildAsync:
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(syncOption), syncOption, null);
            }

            var targetCurrentCodeFrame = currentCodeFrame;

            var currentProcessInfo = codeFrame.ProcessInfo;

            var processInfoValue = new ProcessInfoValue(currentProcessInfo);

            if (increaseCurrentFramePosition)
            {
                targetCurrentCodeFrame.CurrentPosition++;
            }

            if (syncOption == SyncOption.ChildAsync)
            {
                targetCurrentCodeFrame.ProcessInfo.AddChild(logger, currentProcessInfo);
            }

            var monitorNode = _context.MonitorNode;

            var threadId = monitorNode.CreateThreadId();

            var newLogger = monitorNode.CreateThreadLogger("0FBA9877-ACBB-48F1-8CAF-B73CAF435653", threadId :threadId, parentThreadId: parentThreadId);

            var threadExecutor = new AsyncThreadExecutor(_context, _context.CodeExecutionThreadPool, newLogger);
            threadExecutor.SetCodeFrame(codeFrame);

            var task = threadExecutor.Start();

            if (completeAnnotationSystemEvent != null)
            {
                currentProcessInfo.AddOnCompleteHandler(logger, new ProcessInfoEventHandler(_context, threadId, completeAnnotationSystemEvent, currentCodeFrame, true));
            }

            if (weakCancelAnnotationSystemEvent != null)
            {
                currentProcessInfo.AddOnWeakCanceledHandler(logger, new ProcessInfoEventHandler(_context, threadId, weakCancelAnnotationSystemEvent, currentCodeFrame, true));
            }

            if (cancelAnnotationSystemEvent != null)
            {
                throw new NotImplementedException("22ACDC3F-AE3B-4981-B7DF-ACB3D3FDF20E");
            }

            if (errorAnnotationSystemEvent != null)
            {
                throw new NotImplementedException("5C8CBF29-4135-4FE4-AC12-A326C2A10FC2");
            }

            if (increaseCurrentFramePosition)
            {
                targetCurrentCodeFrame.ValuesStack.Push(processInfoValue);
            }
        }
    }
}
