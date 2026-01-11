/*MIT License

Copyright (c) 2020 - 2026 Sergiy Tolkachov

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

using SymOntoClay.CLI;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Monitor.NLog;

namespace TestSandbox.Handlers
{
    public class CLICommandParserHandler
    {
        private static readonly IMonitorLogger _logger = new MonitorLoggerNLogImplementation();

        public void Run()
        {
            _logger.Info("2B26F375-9896-447B-8D4F-1596EE15FB1A", "Begin");

            var commandLine = "";

            _logger.Info("D3BD027C-DC4D-40CB-A0A8-C3F7D117B2D5", $"commandLine = '{commandLine}'");

            var args = ParseCommandLine(commandLine);

            var command = CLICommandParser.Parse(args);

            _logger.Info("10AFCA78-28A0-4278-91ED-284BE22B800D", $"command = {command}");

            _logger.Info("2C97FCD9-83E7-438D-B5F6-C4D91B5AB170", "End");
        }

        private string[] ParseCommandLine(string value)
        {
            return value.Split(' ');
        }
    }
}
