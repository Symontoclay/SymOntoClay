using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;
using TestSandbox.PlatformImplementations;

namespace TestSandbox.Helpers
{
    public static class TestRunner
    {
        private static readonly IEntityLogger _logger = new LoggerImpementation();

        static TestRunner()
        {
            _logger.Log("Construct");
        }

        public static void Run(string programsText, Action<string> loggersCallBackFun, int timeoutToEnd = 50000)
        {
            _logger.Log($"programsText = {programsText}");
            _logger.Log($"timeoutToEnd = {timeoutToEnd}");


        }
    }
}
