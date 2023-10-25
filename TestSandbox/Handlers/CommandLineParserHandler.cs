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
