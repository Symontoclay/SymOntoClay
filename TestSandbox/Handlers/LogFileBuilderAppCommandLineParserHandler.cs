using NLog;
using SymOntoClay.Monitor.LogFileBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestSandbox.Handlers
{
    public class LogFileBuilderAppCommandLineParserHandler
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public void Run()
        {
            _logger.Info("Begin");

            //MinimalValidNamedCommandline_AbsUrl_ErrorsList();
            //MinimalValidNamedCommandline_AbsUrl_Fail();
            //MinimalValidPositionedCommandline_AbsUrl_ErrorsList();
            MinimalValidPositionedCommandline_AbsUrl_Fail();
            //MinimalValidNamedCommandline_Help_ErrorsList();
            //MinimalValidNamedCommandline_Help_Fail();
            //MinimalValidPositionedCommandline_Help_ErrorsList();
            //MinimalValidPositionedCommandline_Help_Fail();
            //EmptyCommandLine_Success();
            //FullValidNamedCommandLine_Success();
            //FullValidPositionedCommandLine_Success();
            //ValidNamedCommandLine_Help_Success();
            //ValidNamedCommandLine_Html_AbsUrl_Success();
            //ValidPositionedCommandLine_Html_AbsUrl_Success();
            //ValidNamedCommandLine_Html_Success();
            //ValidPositionedCommandLine_Html_Success();
            //ValidNamedCommandLine_Configuration_Success();
            //ValidPositionedCommandLine_Configuration_Success();
            //ValidNamedCommandLine_Nologo_Success();
            //ValidPositionedCommandLine_Nologo_Success();
            //ValidNamedCommandLine_SplitByNodes_SplitByThreads_Success();
            //ValidPositionedCommandLine_SplitByNodes_SplitByThreads_Success();
            //ValidNamedCommandLine_TargetNodeId_TargetTreadId_Success();
            //ValidPositionedCommandLine_TargetNodeId_TargetTreadId_Success();
            //MinimalValidNamedCommandline_Success();
            //MinimalValidPositionedCommandline_Success();

            _logger.Info("End");
        }

        private void MinimalValidNamedCommandline_AbsUrl_ErrorsList()
        {
            var args = new List<string>()
                {
                    "--i",
                    @"%USERPROFILE%\SomeInputDir\",
                    "--o",
                    @"%USERPROFILE%\SomeOutputDir\",
                    "--abs-url"
                };

            var parser = new LogFileBuilderAppCommandLineParser(true);

            var result = parser.Parse(args.ToArray());

            _logger.Info($"result = {result}");
            _logger.Info($"result.Params.Count = {result.Params.Count}");
        }

        private void MinimalValidNamedCommandline_AbsUrl_Fail()
        {
            try
            {
                var args = new List<string>()
                {
                    "--i",
                    @"%USERPROFILE%\SomeInputDir\",
                    "--o",
                    @"%USERPROFILE%\SomeOutputDir\",
                    "--abs-url"
                };

                var parser = new LogFileBuilderAppCommandLineParser(false);

                var result = parser.Parse(args.ToArray());

                _logger.Info($"result = {result}");
                _logger.Info($"result.Params.Count = {result.Params.Count}");
            }
            catch (Exception e)
            {
                _logger.Info($"e.Message = '{e.Message}'");
                _logger.Info(e);
            }            
        }

        private void MinimalValidPositionedCommandline_AbsUrl_ErrorsList()
        {
            var args = new List<string>()
                {
                    @"%USERPROFILE%\SomeInputDir\",
                    @"%USERPROFILE%\SomeOutputDir\",
                    "--abs-url"
                };

            var parser = new LogFileBuilderAppCommandLineParser(true);

            var result = parser.Parse(args.ToArray());

            _logger.Info($"result = {result}");
            _logger.Info($"result.Params.Count = {result.Params.Count}");
        }

        private void MinimalValidPositionedCommandline_AbsUrl_Fail()
        {
            try
            {
                var args = new List<string>()
                {
                    @"%USERPROFILE%\SomeInputDir\",
                    @"%USERPROFILE%\SomeOutputDir\",
                    "--abs-url"
                };

                var parser = new LogFileBuilderAppCommandLineParser(false);

                var result = parser.Parse(args.ToArray());

                _logger.Info($"result = {result}");
                _logger.Info($"result.Params.Count = {result.Params.Count}");
            }
            catch (Exception e)
            {
                _logger.Info($"e.Message = '{e.Message}'");
                _logger.Info(e);
            }
        }

        private void MinimalValidNamedCommandline_Help_ErrorsList()
        {
            var args = new List<string>()
                {
                    "--i",
                    @"%USERPROFILE%\SomeInputDir\",
                    "--o",
                    @"%USERPROFILE%\SomeOutputDir\",
                    "--help"
                };

            var parser = new LogFileBuilderAppCommandLineParser(true);

            var result = parser.Parse(args.ToArray());

            _logger.Info($"result = {result}");
            _logger.Info($"result.Params.Count = {result.Params.Count}");
        }

        private void MinimalValidNamedCommandline_Help_Fail()
        {
            try
            {
                var args = new List<string>()
                {
                    "--i",
                    @"%USERPROFILE%\SomeInputDir\",
                    "--o",
                    @"%USERPROFILE%\SomeOutputDir\",
                    "--help"
                };

                var parser = new LogFileBuilderAppCommandLineParser(false);

                var result = parser.Parse(args.ToArray());

                _logger.Info($"result = {result}");
                _logger.Info($"result.Params.Count = {result.Params.Count}");
            }
            catch (Exception e)
            {
                _logger.Info($"e.Message = '{e.Message}'");
                _logger.Info(e);
            }
        }

        private void MinimalValidPositionedCommandline_Help_ErrorsList()
        {
            var args = new List<string>()
                {
                    @"%USERPROFILE%\SomeInputDir\",
                    @"%USERPROFILE%\SomeOutputDir\",
                    "--help"
                };

            var parser = new LogFileBuilderAppCommandLineParser(true);

            var result = parser.Parse(args.ToArray());

            _logger.Info($"result = {result}");
            _logger.Info($"result.Params.Count = {result.Params.Count}");
        }

        private void MinimalValidPositionedCommandline_Help_Fail()
        {
            try
            {
                var args = new List<string>()
                {
                    @"%USERPROFILE%\SomeInputDir\",
                    @"%USERPROFILE%\SomeOutputDir\",
                    "--help"
                };

                var parser = new LogFileBuilderAppCommandLineParser(false);

                var result = parser.Parse(args.ToArray());

                _logger.Info($"result = {result}");
                _logger.Info($"result.Params.Count = {result.Params.Count}");
            }
            catch (Exception e)
            {
                _logger.Info($"e.Message = '{e.Message}'");
                _logger.Info(e);
            }
        }

        private void EmptyCommandLine_Success()
        {
            var args = new List<string>();

            var parser = new LogFileBuilderAppCommandLineParser(true);

            var result = parser.Parse(args.ToArray());

            _logger.Info($"result = {result}");
            _logger.Info($"result.Params.Count = {result.Params.Count}");
        }

        private void FullValidNamedCommandLine_Success()
        {
            var args = new List<string>()
                {
                    "--i",
                    @"%USERPROFILE%\SomeInputDir\",
                    "--o",
                    @"%USERPROFILE%\SomeOutputDir\",
                    "--target-nodeid",
                    "#DummyNPC",
                    "--target-threadid",
                    "#020ED339-6313-459A-900D-92F809CEBDC5",
                    "--split-by-nodes",
                    "--split-by-threads",
                    "--nologo",
                    "--configuration",
                    @"%USERPROFILE%\Some.config",
                    "--html",
                    "--abs-url"
                };

            var parser = new LogFileBuilderAppCommandLineParser(true);

            var result = parser.Parse(args.ToArray());

            _logger.Info($"result = {result}");
            _logger.Info($"result.Params.Count = {result.Params.Count}");
        }

        private void FullValidPositionedCommandLine_Success()
        {
            var args = new List<string>()
                {
                    @"%USERPROFILE%\SomeInputDir\",
                    @"%USERPROFILE%\SomeOutputDir\",
                    "--target-nodeid",
                    "#DummyNPC",
                    "--target-threadid",
                    "#020ED339-6313-459A-900D-92F809CEBDC5",
                    "--split-by-nodes",
                    "--split-by-threads",
                    "--nologo",
                    "--configuration",
                    @"%USERPROFILE%\Some.config",
                    "--html",
                    "--abs-url"
                };

            var parser = new LogFileBuilderAppCommandLineParser(true);

            var result = parser.Parse(args.ToArray());

            _logger.Info($"result = {result}");
            _logger.Info($"result.Params.Count = {result.Params.Count}");
        }

        private void ValidNamedCommandLine_Help_Success()
        {
            var args = new List<string>()
            {
                "--help"
            };

            var parser = new LogFileBuilderAppCommandLineParser(true);

            var result = parser.Parse(args.ToArray());

            _logger.Info($"result = {result}");
        }

        private void ValidNamedCommandLine_Html_AbsUrl_Success()
        {
            var args = new List<string>()
                {
                    "--i",
                    @"%USERPROFILE%\SomeInputDir\",
                    "--o",
                    @"%USERPROFILE%\SomeOutputDir\",
                    "--html",
                    "--abs-url"
                };

            var parser = new LogFileBuilderAppCommandLineParser(true);

            var result = parser.Parse(args.ToArray());

            _logger.Info($"result = {result}");
        }

        private void ValidPositionedCommandLine_Html_AbsUrl_Success()
        {
            var args = new List<string>()
                {
                    @"%USERPROFILE%\SomeInputDir\",
                    @"%USERPROFILE%\SomeOutputDir\",
                    "--html",
                    "--abs-url"
                };

            var parser = new LogFileBuilderAppCommandLineParser(true);

            var result = parser.Parse(args.ToArray());

            _logger.Info($"result = {result}");
        }

        private void ValidNamedCommandLine_Html_Success()
        {
            var args = new List<string>()
                {
                    "--i",
                    @"%USERPROFILE%\SomeInputDir\",
                    "--o",
                    @"%USERPROFILE%\SomeOutputDir\",
                    "--html"
                };

            var parser = new LogFileBuilderAppCommandLineParser(true);

            var result = parser.Parse(args.ToArray());

            _logger.Info($"result = {result}");
        }

        private void ValidPositionedCommandLine_Html_Success()
        {
            var args = new List<string>()
                {
                    @"%USERPROFILE%\SomeInputDir\",
                    @"%USERPROFILE%\SomeOutputDir\",
                    "--html"
                };

            var parser = new LogFileBuilderAppCommandLineParser(true);

            var result = parser.Parse(args.ToArray());

            _logger.Info($"result = {result}");
        }

        private void ValidNamedCommandLine_Configuration_Success()
        {
            var args = new List<string>()
                {
                    "--i",
                    @"%USERPROFILE%\SomeInputDir\",
                    "--o",
                    @"%USERPROFILE%\SomeOutputDir\",
                    "--configuration",
                    @"%USERPROFILE%\Some.config"
                };

            var parser = new LogFileBuilderAppCommandLineParser(true);

            var result = parser.Parse(args.ToArray());

            _logger.Info($"result = {result}");
        }

        private void ValidPositionedCommandLine_Configuration_Success()
        {
            var args = new List<string>()
                {
                    @"%USERPROFILE%\SomeInputDir\",
                    @"%USERPROFILE%\SomeOutputDir\",
                    "--configuration",
                    @"%USERPROFILE%\Some.config"
                };

            var parser = new LogFileBuilderAppCommandLineParser(true);

            var result = parser.Parse(args.ToArray());

            _logger.Info($"result = {result}");
        }

        private void ValidNamedCommandLine_Nologo_Success()
        {
            var args = new List<string>()
                {
                    "--i",
                    @"%USERPROFILE%\SomeInputDir\",
                    "--o",
                    @"%USERPROFILE%\SomeOutputDir\",
                    "--nologo"
                };

            var parser = new LogFileBuilderAppCommandLineParser(true);

            var result = parser.Parse(args.ToArray());

            _logger.Info($"result = {result}");
        }

        private void ValidPositionedCommandLine_Nologo_Success()
        {
            var args = new List<string>()
                {
                    @"%USERPROFILE%\SomeInputDir\",
                    @"%USERPROFILE%\SomeOutputDir\",
                    "--nologo"
                };

            var parser = new LogFileBuilderAppCommandLineParser(true);

            var result = parser.Parse(args.ToArray());

            _logger.Info($"result = {result}");
        }

        private void ValidNamedCommandLine_SplitByNodes_SplitByThreads_Success()
        {
            var args = new List<string>()
                {
                    "--i",
                    @"%USERPROFILE%\SomeInputDir\",
                    "--o",
                    @"%USERPROFILE%\SomeOutputDir\",
                    "--split-by-nodes",
                    "--split-by-threads"
                };

            var parser = new LogFileBuilderAppCommandLineParser(true);

            var result = parser.Parse(args.ToArray());

            _logger.Info($"result = {result}");
        }

        private void ValidPositionedCommandLine_SplitByNodes_SplitByThreads_Success()
        {
            var args = new List<string>()
                {
                    @"%USERPROFILE%\SomeInputDir\",
                    @"%USERPROFILE%\SomeOutputDir\",
                    "--split-by-nodes",
                    "--split-by-threads"
                };

            var parser = new LogFileBuilderAppCommandLineParser(true);

            var result = parser.Parse(args.ToArray());

            _logger.Info($"result = {result}");
        }

        private void ValidNamedCommandLine_TargetNodeId_TargetTreadId_Success()
        {
            var args = new List<string>()
                {
                    "--i",
                    @"%USERPROFILE%\SomeInputDir\",
                    "--o",
                    @"%USERPROFILE%\SomeOutputDir\",
                    "--target-nodeid",
                    "#DummyNPC",
                    "--target-threadid",
                    "#020ED339-6313-459A-900D-92F809CEBDC5"
                };

            var parser = new LogFileBuilderAppCommandLineParser(true);

            var result = parser.Parse(args.ToArray());

            _logger.Info($"result = {result}");
        }

        private void ValidPositionedCommandLine_TargetNodeId_TargetTreadId_Success()
        {
            var args = new List<string>()
                {
                    @"%USERPROFILE%\SomeInputDir\",
                    @"%USERPROFILE%\SomeOutputDir\",
                    "--target-nodeid",
                    "#DummyNPC",
                    "--target-threadid",
                    "#020ED339-6313-459A-900D-92F809CEBDC5"
                };

            var parser = new LogFileBuilderAppCommandLineParser(true);

            var result = parser.Parse(args.ToArray());

            _logger.Info($"result = {result}");
        }

        private void MinimalValidNamedCommandline_Success()
        {
            var args = new List<string>()
                {
                    "--i",
                    @"%USERPROFILE%\SomeInputDir\",
                    "--o",
                    @"%USERPROFILE%\SomeOutputDir\"
                };

            var parser = new LogFileBuilderAppCommandLineParser(true);

            var result = parser.Parse(args.ToArray());

            _logger.Info($"result = {result}");
        }

        private void MinimalValidPositionedCommandline_Success()
        {
            var args = new List<string>()
                {
                    @"%USERPROFILE%\SomeInputDir\",
                    @"%USERPROFILE%\SomeOutputDir\"
                };

            var parser = new LogFileBuilderAppCommandLineParser(true);

            var result = parser.Parse(args.ToArray());

            _logger.Info($"result = {result}");
        }
    }
}
