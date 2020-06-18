using NLog;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.IndexedData.ScriptingData;
using System;
using System.Collections.Generic;
using System.Text;

namespace TestSandbox.CodeExecution
{
    public class CodeExecutionHandler
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public void Run()
        {
            _logger.Info("Begin");

            var command = new ScriptCommand();

            _logger.Info($"command = {command}");

            var compiledFunctionBody = new CompiledFunctionBody();
            compiledFunctionBody.Commands[0] = command;

            _logger.Info($"compiledFunctionBody = {compiledFunctionBody}");

            var codeFrame = new CodeFrame();
            codeFrame.CompiledFunctionBody = compiledFunctionBody;


            _logger.Info($"codeFrame = {codeFrame}");

            _logger.Info("End");
        }
    }
}
