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
#if DEBUG
            //Log($"processInitialInfoList = {processInitialInfoList.WriteListToString()}");
#endif

            var codeFramesList = new List<CodeFrame>();

            foreach(var processInitialInfo in processInitialInfoList)
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
            _context.InstancesStorage.PrintProcessesList();
#endif

            var threadExecutor = new AsyncThreadExecutor(_context);
            threadExecutor.SetCodeFrames(codeFramesList);

            return threadExecutor.Start();
        }
    }
}
