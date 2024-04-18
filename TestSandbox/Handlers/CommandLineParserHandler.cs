/*MIT License

Copyright (c) 2020 - 2024 Sergiy Tolkachov

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

using Newtonsoft.Json;
using NLog;
using SymOntoClay.CLI.Helpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestSandbox.Handlers
{
    public class CommandLineParserHandler
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public void Run()
        {
            _logger.Info("Begin");

            Case5();
            //Case4();
            //Case3();
            //Case2();
            //Case1();

            _logger.Info("End");
        }

        private void Case5()
        {
            var cmdArg = @"c:\Users\sergiy.tolkachov\source\repos\ --nologo";

            _logger.Info($"cmdArg = '{cmdArg}'");

            var cmdStrList = cmdArg.Split(' ');

            _logger.Info($"cmdStrList = '{cmdStrList.WritePODListToString()}'");

            var parser = new CommandLineParser();
            parser.RegisterArgument(new CommandLineArgumentOptions
            {
                Name = "--input",
                Aliases = new List<string>
                {
                    "--i"
                },
                Kind = KindOfCommandLineArgument.SingleValue,
                IsDefault = false
            });

            parser.RegisterArgument(new CommandLineArgumentOptions
            {
                Name = "--output",
                Aliases = new List<string>
                {
                    "--o"
                },
                Kind = KindOfCommandLineArgument.SingleValue,
                IsDefault = false
            });

            parser.RegisterArgument(new CommandLineArgumentOptions
            {
                Name = "--nologo",
                Kind = KindOfCommandLineArgument.Flag
            });

            var result = parser.Parse(cmdStrList);

            _logger.Info($"result = {JsonConvert.SerializeObject(result, Formatting.Indented)}");
        }

        private void Case4()
        {
            var cmdArg = @"c:\Users\sergiy.tolkachov\source\repos\ --nologo";

            _logger.Info($"cmdArg = '{cmdArg}'");

            var cmdStrList = cmdArg.Split(' ');

            _logger.Info($"cmdStrList = '{cmdStrList.WritePODListToString()}'");

            var parser = new CommandLineParser();
            parser.RegisterArgument(new CommandLineArgumentOptions
            {
                Name = "--input",
                Aliases = new List<string>
                {
                    "--i"
                },
                Kind = KindOfCommandLineArgument.SingleValue,
                IsDefault = true
            });

            parser.RegisterArgument(new CommandLineArgumentOptions
            {
                Name = "--output",
                Aliases = new List<string>
                {
                    "--o"
                },
                Kind = KindOfCommandLineArgument.SingleValue,
                IsDefault = true
            });

            parser.RegisterArgument(new CommandLineArgumentOptions
            {
                Name = "--nologo",
                Kind = KindOfCommandLineArgument.Flag
            });

            var result = parser.Parse(cmdStrList);

            _logger.Info($"result = {JsonConvert.SerializeObject(result, Formatting.Indented)}");
        }

        private void Case3()
        {
            var cmdArg = @"--nologo c:\Users\sergiy.tolkachov\source\repos\";

            _logger.Info($"cmdArg = '{cmdArg}'");

            var cmdStrList = cmdArg.Split(' ');

            _logger.Info($"cmdStrList = '{cmdStrList.WritePODListToString()}'");

            var parser = new CommandLineParser();
            parser.RegisterArgument(new CommandLineArgumentOptions
            {
                Name = "--input",
                Aliases = new List<string>
                {
                    "--i"
                },
                Kind = KindOfCommandLineArgument.SingleValue,
                IsDefault = true
            });

            parser.RegisterArgument(new CommandLineArgumentOptions
            {
                Name = "--nologo",
                Kind = KindOfCommandLineArgument.Flag
            });

            var result = parser.Parse(cmdStrList);

            _logger.Info($"result = {JsonConvert.SerializeObject(result, Formatting.Indented)}");
        }

        private void Case2()
        {
            var cmdArg = @"c:\Users\sergiy.tolkachov\source\repos\ --nologo";

            _logger.Info($"cmdArg = '{cmdArg}'");

            var cmdStrList = cmdArg.Split(' ');

            _logger.Info($"cmdStrList = '{cmdStrList.WritePODListToString()}'");

            var parser = new CommandLineParser();
            parser.RegisterArgument(new CommandLineArgumentOptions
            {
                Name = "--input",
                Aliases = new List<string>
                {
                    "--i"
                },
                Kind = KindOfCommandLineArgument.SingleValue,
                IsDefault = true
            });

            parser.RegisterArgument(new CommandLineArgumentOptions
            {
                Name = "--nologo",
                Kind = KindOfCommandLineArgument.Flag
            });

            var result = parser.Parse(cmdStrList);

            _logger.Info($"result = {JsonConvert.SerializeObject(result, Formatting.Indented)}");
        }

        private void Case1()
        {
            var cmdArg = @"--i c:\Users\sergiy.tolkachov\source\repos\ --nologo";

            _logger.Info($"cmdArg = '{cmdArg}'");

            var cmdStrList = cmdArg.Split(' ');

            _logger.Info($"cmdStrList = '{cmdStrList.WritePODListToString()}'");

            var parser = new CommandLineParser();
            parser.RegisterArgument(new CommandLineArgumentOptions
            {
                Name = "--input",
                Aliases = new List<string>
                {
                    "--i"
                },
                Kind = KindOfCommandLineArgument.SingleValue,
                IsDefault = true
            });

            parser.RegisterArgument(new CommandLineArgumentOptions
            {
                Name = "--nologo",
                Kind = KindOfCommandLineArgument.Flag
            });

            var result = parser.Parse(cmdStrList);

            _logger.Info($"result = {JsonConvert.SerializeObject(result, Formatting.Indented)}");
        }
    }
}
