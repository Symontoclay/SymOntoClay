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
        public ProcessInfoEventHandler(IEngineContext context, string parentThreadId, IExecutable handler, CodeFrame currentCodeFrame, bool runOnce)
            : base(context.Logger)
        {
            _parentThreadId = parentThreadId;
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

        private readonly string _parentThreadId;

        private IExecutable _handler;
        private CodeFrame _currentCodeFrame;
        private readonly bool _runOnce;
        private bool _isAlreadyRun;

        private readonly object _lockObj = new object();

        /// <inheritdoc/>
        public void Run()
        {
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

            _codeFrameAsyncExecutor.AsyncExecuteCodeFrame(_parentThreadId, newCodeFrame, _currentCodeFrame, lifeCycleEventCoordinator, SyncOption.ChildAsync, false);
        }
    }
}
