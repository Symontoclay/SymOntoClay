/*MIT License

Copyright (c) 2020 - 2023 Sergiy Tolkachov

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
using SymOntoClay.Core.Internal.Services;
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

        /// <inheritdoc/>
        public void AsyncExecuteCodeFrame(CodeFrame codeFrame, IExecutionCoordinator coordinator, SyncOption syncOption, bool increaceCurrentFramePosition = true, AnnotationSystemEvent completeAnnotationSystemEvent = null, AnnotationSystemEvent cancelAnnotationSystemEvent = null, AnnotationSystemEvent weakCancelAnnotationSystemEvent = null, AnnotationSystemEvent errorAnnotationSystemEvent = null)
        {
            AsyncExecuteCodeFrame(codeFrame, null, coordinator, syncOption, increaceCurrentFramePosition, completeAnnotationSystemEvent, cancelAnnotationSystemEvent, weakCancelAnnotationSystemEvent, errorAnnotationSystemEvent);
        }

        /// <inheritdoc/>
        public void AsyncExecuteCodeFrame(CodeFrame codeFrame, CodeFrame currentCodeFrame, IExecutionCoordinator coordinator, SyncOption syncOption, bool increaceCurrentFramePosition = true, AnnotationSystemEvent completeAnnotationSystemEvent = null, AnnotationSystemEvent cancelAnnotationSystemEvent = null, AnnotationSystemEvent weakCancelAnnotationSystemEvent = null, AnnotationSystemEvent errorAnnotationSystemEvent = null)
        {
            AsyncExecuteCodeFrame(string.Empty, codeFrame, currentCodeFrame, coordinator, syncOption, increaceCurrentFramePosition, completeAnnotationSystemEvent, AnnotationSystemEvent cancelAnnotationSystemEvent = null, AnnotationSystemEvent weakCancelAnnotationSystemEvent = null, AnnotationSystemEvent errorAnnotationSystemEvent = null);
        }

        public void AsyncExecuteCodeFrame(string parentThreadId, CodeFrame codeFrame, CodeFrame currentCodeFrame, IExecutionCoordinator coordinator, SyncOption syncOption, bool increaceCurrentFramePosition = true, AnnotationSystemEvent completeAnnotationSystemEvent = null, AnnotationSystemEvent cancelAnnotationSystemEvent = null, AnnotationSystemEvent weakCancelAnnotationSystemEvent = null, AnnotationSystemEvent errorAnnotationSystemEvent = null)
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

            if (increaceCurrentFramePosition)
            {
                targetCurrentCodeFrame.CurrentPosition++;
            }

            if (syncOption == SyncOption.ChildAsync)
            {
                targetCurrentCodeFrame.ProcessInfo.AddChild(currentProcessInfo);
            }

            var threadExecutor = new AsyncThreadExecutor(_context);
            threadExecutor.SetCodeFrame(codeFrame);

            var task = threadExecutor.Start();

            if (completeAnnotationSystemEvent != null)
            {
                currentProcessInfo.AddOnCompleteHandler(new ProcessInfoEventHandler(_context, completeAnnotationSystemEvent, currentCodeFrame, true));
            }

            if (weakCancelAnnotationSystemEvent != null)
            {
                currentProcessInfo.AddOnWeakCanceledHandler(new ProcessInfoEventHandler(_context, weakCancelAnnotationSystemEvent, currentCodeFrame, true));
            }

            if (cancelAnnotationSystemEvent != null)
            {
                throw new NotImplementedException();
            }

            if (errorAnnotationSystemEvent != null)
            {
                throw new NotImplementedException();
            }

            if (increaceCurrentFramePosition)
            {
                targetCurrentCodeFrame.ValuesStack.Push(processInfoValue);
            }
        }
    }
}
