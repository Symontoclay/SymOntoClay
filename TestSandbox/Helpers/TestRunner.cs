using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.Unity3DAsset.Test.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TestSandbox.PlatformImplementations;

namespace TestSandbox.Helpers
{
    public static class TestRunner
    {
        private static readonly IEntityLogger _logger = new LoggerImpementation();

        public static void Run(string fileContent, Action<string> loggersCallBackFun, int timeoutToEnd = 5000)
        {
            _logger.Log($"fileContent = {fileContent}");
            _logger.Log($"timeoutToEnd = {timeoutToEnd}");

            var behaviorTestEngineInstance = new BehaviorTestEngineInstance();

            behaviorTestEngineInstance.WriteFile(fileContent);

            behaviorTestEngineInstance.Run(timeoutToEnd,
                message => { _logger.Log($"message = {message}"); },
                error => { _logger.Log($"error = {error}"); }
                );
        }
    }
}
