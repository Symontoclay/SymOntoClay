using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.CoreHelper.Threads;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeExecution
{
    public abstract class BaseThreadExecutor: BaseLoggedComponent
    {
        protected BaseThreadExecutor(IEntityLogger logger, IActivePeriodicObject activeObject)
            :base(logger)
        {
        }

        private Stack<CodeFrame> _codeFrames = new Stack<CodeFrame>();
        private CodeFrame _currentCodeFrame;


    }
}
