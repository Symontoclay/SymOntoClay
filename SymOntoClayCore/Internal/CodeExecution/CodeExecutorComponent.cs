using SymOntoClay.Core.Internal.CodeModel;
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
        public Value ExecuteBatchAsync(List<CodeFrame> codeFramesList)
        {
#if DEBUG
            //Log($"codeFramesList = {codeFramesList.WriteListToString()}");
#endif

            var threadExecutor = new AsyncThreadExecutor(_context);
            threadExecutor.SetCodeFrames(codeFramesList);

            return threadExecutor.Start();
        }
    }
}
