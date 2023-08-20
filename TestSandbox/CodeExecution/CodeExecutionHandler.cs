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
using SymOntoClay.Monitor.Common;
using SymOntoClay.Monitor.NLog;
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
        private static readonly IMonitorLogger _logger = new MonitorLoggerNLogImpementation();

        public void Run()
        {
            _logger.Info("0B8F0BCD-44D2-4000-9790-F782E90975EC", "Begin");

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

            _logger.Info("3DAB2E7B-0380-431F-924A-33BEE41E3F35", $"command = {command}");

            compiledFunctionBody.Commands[0] = command;

            var identifier = NameHelper.CreateName("@>log");

            command = new ScriptCommand();
            command.OperationCode = OperationCode.PushVal;
            command.Value = identifier;
            command.Position = 1;

            _logger.Info("972642DC-1B73-4713-A30A-02C030936DE9", $"command = {command}");

            compiledFunctionBody.Commands[1] = command;


            command = new ScriptCommand();
            command.OperationCode = OperationCode.CallBinOp;
            command.KindOfOperator = KindOfOperator.LeftRightStream;
            command.Position = 2;

            _logger.Info("96A522D7-2A66-4838-BC2F-B429391E0AEC", $"command = {command}");

            compiledFunctionBody.Commands[2] = command;

            command = new ScriptCommand();
            command.OperationCode = OperationCode.Return;
            command.Position = 3;

            _logger.Info("24199F57-28C9-4A08-A8F5-09ACBA24BBEC", $"command = {command}");

            compiledFunctionBody.Commands[3] = command;

            _logger.Info("8FCCA2FF-90E3-426D-879B-46F0FBBCD316", $"compiledFunctionBody = {compiledFunctionBody}");
            _logger.Info("550A8C8E-1F86-4444-B577-856BE9BF7057", $"compiledFunctionBody = {compiledFunctionBody.ToDbgString()}");

            var codeFrame = new CodeFrame();
            codeFrame.CompiledFunctionBody = compiledFunctionBody;

            var codeFrameLocalContext = new LocalCodeExecutionContext();
            codeFrameLocalContext.Storage = context.Storage.GlobalStorage;
            codeFrameLocalContext.Holder = NameHelper.CreateName("PixKeeper");

            codeFrame.LocalContext = codeFrameLocalContext;

            _logger.Info("8DCC3C1B-B548-4FBB-B6EB-A0086AF10601", $"codeFrame = {codeFrame}");
            _logger.Info("621EE055-AFB2-4B1E-BDC7-17D999C62329", $"codeFrame = {codeFrame.ToDbgString()}");

            var threadExecutor = new SyncThreadExecutor(context);

            threadExecutor.SetCodeFrame(codeFrame);

            threadExecutor.Start();

            _logger.Info("078C6419-54F8-46FE-A11B-DD50DE4FE201", "End");
        }
    }
}
