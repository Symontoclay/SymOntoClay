using SymOntoClay.CLI;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestSandbox.PlatformImplementations;

namespace TestSandbox.Handlers
{
    public class CLICommandParserHandler
    {
        private static readonly IEntityLogger _logger = new LoggerImpementation();

        public void Run()
        {
            _logger.Log("Begin");

            var commandLine = "";

            _logger.Log($"commandLine = '{commandLine}'");

            var args = ParseCommandLine(commandLine);

            var command = CLICommandParser.Parse(args);

            _logger.Log($"command = {command}");

            _logger.Log("End");
        }

        private string[] ParseCommandLine(string value)
        {
            return value.Split(' ');
        }
    }
}
