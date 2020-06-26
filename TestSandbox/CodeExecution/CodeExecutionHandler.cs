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

            var context = TstEngineContextHelper.CreateAndInitContext();

            var compiledFunctionBody = new CompiledFunctionBody();
            
            var strVal = new StringValue("The beatles!");

            var command = new ScriptCommand();
            command.OperationCode = OperationCode.PushVal;
            command.Value = strVal;

            _logger.Log($"command = {command}");

            compiledFunctionBody.Commands[0] = command;

            var identifier = NameHelper.CreateName("@>log", context.Dictionary);

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

            _logger.Log($"compiledFunctionBody = {compiledFunctionBody}");
            _logger.Log($"compiledFunctionBody = {compiledFunctionBody.ToDbgString()}");

            var codeFrame = new CodeFrame();
            codeFrame.CompiledFunctionBody = compiledFunctionBody;
            codeFrame.Storage = context.Storage.GlobalStorage;

            _logger.Log($"codeFrame = {codeFrame}");
            _logger.Log($"codeFrame = {codeFrame.ToDbgString()}");

            var threadExecutor = new SyncThreadExecutor(context);

            threadExecutor.SetCodeFrame(codeFrame);

            threadExecutor.Start();

            _logger.Log("End");
        }
    }
}
