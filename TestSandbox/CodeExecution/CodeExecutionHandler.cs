using NLog;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Ast.Expressions;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Core.Internal.IndexedData.ScriptingData;
using System;
using System.Collections.Generic;
using System.Text;
using TestSandbox.Parsing;

namespace TestSandbox.CodeExecution
{
    public class CodeExecutionHandler
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public void Run()
        {
            _logger.Info("Begin");

            var parserContext = new TstParserContext();

            var compiledFunctionBody = new CompiledFunctionBody();
            
            var strVal = new StringValue("The beatles!");

            var command = new ScriptCommand();
            command.OperationCode = OperationCode.PushVal;
            command.Value = strVal;

            _logger.Info($"command = {command}");

            compiledFunctionBody.Commands[0] = command;

            var identifier = NameHelper.CreateName("@>log", parserContext.Dictionary);

            command = new ScriptCommand();
            command.OperationCode = OperationCode.PushVal;
            command.Value = identifier;
            command.Position = 1;

            _logger.Info($"command = {command}");

            compiledFunctionBody.Commands[1] = command;


            command = new ScriptCommand();
            command.OperationCode = OperationCode.CallBinOp;
            command.KindOfOperator = KindOfOperator.LeftRightStream;
            command.Position = 2;

            _logger.Info($"command = {command}");

            compiledFunctionBody.Commands[2] = command;

            _logger.Info($"compiledFunctionBody = {compiledFunctionBody}");

            var codeFrame = new CodeFrame();
            codeFrame.CompiledFunctionBody = compiledFunctionBody;

            _logger.Info($"codeFrame = {codeFrame}");

            var threadExecutor = new SyncThreadExecutor(parserContext.Logger);

            threadExecutor.SetCodeFrame(codeFrame);

            threadExecutor.Start();

            _logger.Info("End");
        }
    }
}
