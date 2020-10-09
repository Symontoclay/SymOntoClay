/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

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
