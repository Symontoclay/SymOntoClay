using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeExecution
{
    public class ThreadExecutor: BaseLoggedComponent
    {
        public ThreadExecutor(IEntityLogger logger)
            :base(logger)
        {
        }

        private Stack<CodeFrame> _codeFrames = new Stack<CodeFrame>();
        private CodeFrame _currentCodeFrame;


    }
}
