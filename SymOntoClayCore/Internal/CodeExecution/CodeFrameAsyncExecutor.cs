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
