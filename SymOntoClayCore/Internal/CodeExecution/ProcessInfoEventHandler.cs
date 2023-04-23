using SymOntoClay.Core.Internal.CodeExecution.Helpers;
using SymOntoClay.Core.Internal.Services;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeExecution
{
    public class ProcessInfoEventHandler: BaseComponent, IProcessInfoEventHandler
    {
        public ProcessInfoEventHandler(IEngineContext context, IExecutable handler, CodeFrame currentCodeFrame, bool runOnce)
            : base(context.Logger)
        {
            _context = context;
            _codeFrameService = context.ServicesFactory.GetCodeFrameService();
            _codeFrameAsyncExecutor = new CodeFrameAsyncExecutor(context);
            _handler = handler;
            _currentCodeFrame = currentCodeFrame;
            _runOnce = runOnce;
        }

        /// <inheritdoc/>
        public IProcessInfo ProcessInfo { get; set; }

        private readonly IEngineContext _context;
        private readonly ICodeFrameService _codeFrameService;

        private readonly CodeFrameAsyncExecutor _codeFrameAsyncExecutor;

        private IExecutable _handler;
        private CodeFrame _currentCodeFrame;
        private readonly bool _runOnce;
        private bool _isAlreadyRun;

        private readonly object _lockObj = new object();

        /// <inheritdoc/>
        public void Run()
        {
#if DEBUG
            //Log("Begin");
#endif

            lock(_lockObj)
            {
                if(_runOnce)
                {
                    if(_isAlreadyRun)
                    {
                        return;
                    }

                    _isAlreadyRun = true;
                }                
            }

            var lifeCycleEventCoordinator = _handler.GetCoordinator(_context, _currentCodeFrame.LocalContext);
            var newCodeFrame = _codeFrameService.ConvertExecutableToCodeFrame(_handler, KindOfFunctionParameters.NoParameters, null, null, _currentCodeFrame.LocalContext, null, true);

            _codeFrameAsyncExecutor.AsyncExecuteCodeFrame(newCodeFrame, _currentCodeFrame, lifeCycleEventCoordinator, SyncOption.ChildAsync, false);
        }
    }
}
