using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeExecution
{
    public class ProcessInfoEventHandler: BaseComponent, IProcessInfoEventHandler
    {
        public ProcessInfoEventHandler(IEngineContext context, CodeFrame newCodeFrame, CodeFrame currentCodeFrame, IExecutionCoordinator lifeCycleEventCoordinator)
            : base(context.Logger)
        {
            _newCodeFrame = newCodeFrame;
            _currentCodeFrame = currentCodeFrame;
            _lifeCycleEventCoordinator = lifeCycleEventCoordinator;
        }

        /// <inheritdoc/>
        public IProcessInfo ProcessInfo { get; set; }

        private CodeFrame _newCodeFrame;
        private CodeFrame _currentCodeFrame; 
        private IExecutionCoordinator _lifeCycleEventCoordinator;

        /// <inheritdoc/>
        public void Run()
        {
            throw new NotImplementedException();
        }
    }
}
