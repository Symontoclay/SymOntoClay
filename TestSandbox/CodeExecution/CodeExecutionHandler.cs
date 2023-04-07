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

using NLog;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Ast.Expressions;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Core.Internal.IndexedData.ScriptingData;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;
using TestSandbox.Helpers;
using TestSandbox.Parsing;
using TestSandbox.PlatformImplementations;

namespace TestSandbox.CodeExecution
{
    public class CodeExecutionHandler
    {
        private static readonly IEntityLogger _logger = new LoggerImpementation();

        public void Run()
        {
            _logger.Log("Begin");

            var context = TstEngineContextHelper.CreateAndInitContext().EngineContext;

            var applicationInheritanceItem = new InheritanceItem()
            {
                IsSystemDefined = true
            };

            applicationInheritanceItem.SubName = NameHelper.CreateName("PeaseKeeper");
            applicationInheritanceItem.SuperName = context.CommonNamesStorage.AppName;
            applicationInheritanceItem.Rank = new LogicalValue(1.0F);

            context.Storage.GlobalStorage.InheritanceStorage.SetInheritance(applicationInheritanceItem);

            var compiledFunctionBody = new CompiledFunctionBody();
            
            var strVal = new StringValue("The beatles!");

            var command = new ScriptCommand();
            command.OperationCode = OperationCode.PushVal;
            command.Value = strVal;

            _logger.Log($"command = {command}");

            compiledFunctionBody.Commands[0] = command;

            var identifier = NameHelper.CreateName("@>log");

            command = new ScriptCommand();
            command.OperationCode = OperationCode.PushVal;
            command.Value = identifier;
            command.Position = 1;

            _logger.Log($"command = {command}");

            compiledFunctionBody.Commands[1] = command;


            command = new ScriptCommand();
            command.OperationCode = OperationCode.CallBinOp;
            command.KindOfOperator = KindOfOperator.LeftRightStream;
            command.Position = 2;

            _logger.Log($"command = {command}");

            compiledFunctionBody.Commands[2] = command;

            command = new ScriptCommand();
            command.OperationCode = OperationCode.Return;
            command.Position = 3;

            _logger.Log($"command = {command}");

            compiledFunctionBody.Commands[3] = command;

            _logger.Log($"compiledFunctionBody = {compiledFunctionBody}");
            _logger.Log($"compiledFunctionBody = {compiledFunctionBody.ToDbgString()}");

            var codeFrame = new CodeFrame();
            codeFrame.CompiledFunctionBody = compiledFunctionBody;

            var codeFrameLocalContext = new LocalCodeExecutionContext();
            codeFrameLocalContext.Storage = context.Storage.GlobalStorage;
            codeFrameLocalContext.Holder = NameHelper.CreateName("PixKeeper");

            codeFrame.LocalContext = codeFrameLocalContext;

            _logger.Log($"codeFrame = {codeFrame}");
            _logger.Log($"codeFrame = {codeFrame.ToDbgString()}");

            var threadExecutor = new SyncThreadExecutor(context);

            threadExecutor.SetCodeFrame(codeFrame);

            threadExecutor.Start();

            _logger.Log("End");
        }
    }
}
