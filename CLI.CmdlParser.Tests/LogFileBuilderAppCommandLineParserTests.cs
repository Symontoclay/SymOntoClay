using SymOntoClay.Monitor.LogFileBuilder;

namespace CLI.CmdlParser.Tests
{
    public class LogFileBuilderAppCommandLineParserTests
    {
        [Test]
        public void MinimalValidPositionedCommandline_Success()
        {
            var inputKey = "--input";
            var outputKey = "--output";

            var args = new List<string>()
                {
                    @"%USERPROFILE%\SomeInputDir\",
                    @"%USERPROFILE%\SomeOutputDir\"
                };

            var parser = new LogFileBuilderAppCommandLineParser(true);

            var result = parser.Parse(args.ToArray());

            Assert.That(result.Errors.Count, Is.EqualTo(0));

            Assert.That(result.Params.Count, Is.EqualTo(2));

            Assert.That(result.Params.ContainsKey(inputKey), Is.EqualTo(true));
            Assert.That(result.Params[inputKey], Is.EqualTo(@"%USERPROFILE%\SomeInputDir\"));

            Assert.That(result.Params.ContainsKey(outputKey), Is.EqualTo(true));
            Assert.That(result.Params[outputKey], Is.EqualTo(@"%USERPROFILE%\SomeOutputDir\"));
        }

        [Test]
        public void MinimalValidNamedCommandline_Success()
        {
            var inputKey = "--input";
            var outputKey = "--output";

            var args = new List<string>()
                {
                    "--i",
                    @"%USERPROFILE%\SomeInputDir\",
                    "--o",
                    @"%USERPROFILE%\SomeOutputDir\"
                };

            var parser = new LogFileBuilderAppCommandLineParser(true);

            var result = parser.Parse(args.ToArray());

            Assert.That(result.Errors.Count, Is.EqualTo(0));

            Assert.That(result.Params.Count, Is.EqualTo(2));

            Assert.That(result.Params.ContainsKey(inputKey), Is.EqualTo(true));
            Assert.That(result.Params[inputKey], Is.EqualTo(@"%USERPROFILE%\SomeInputDir\"));

            Assert.That(result.Params.ContainsKey(outputKey), Is.EqualTo(true));
            Assert.That(result.Params[outputKey], Is.EqualTo(@"%USERPROFILE%\SomeOutputDir\"));
        }

        [Test]
        public void ValidPositionedCommandLine_TargetNodeId_TargetTreadId_Success()
        {
            var inputKey = "--input";
            var outputKey = "--output";
            var targetNodeIdKey = "--target-nodeid";
            var targetTreadIdKey = "--target-threadid";

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

            Assert.That(result.Errors.Count, Is.EqualTo(0));

            Assert.That(result.Params.Count, Is.EqualTo(4));

            Assert.That(result.Params.ContainsKey(inputKey), Is.EqualTo(true));
            Assert.That(result.Params[inputKey], Is.EqualTo(@"%USERPROFILE%\SomeInputDir\"));

            Assert.That(result.Params.ContainsKey(outputKey), Is.EqualTo(true));
            Assert.That(result.Params[outputKey], Is.EqualTo(@"%USERPROFILE%\SomeOutputDir\"));

            Assert.That(result.Params.ContainsKey(targetNodeIdKey), Is.EqualTo(true));
            Assert.That(result.Params[targetNodeIdKey], Is.EqualTo("#DummyNPC"));

            Assert.That(result.Params.ContainsKey(targetTreadIdKey), Is.EqualTo(true));
            Assert.That(result.Params[targetTreadIdKey], Is.EqualTo("#020ED339-6313-459A-900D-92F809CEBDC5"));
        }

        [Test]
        public void ValidNamedCommandLine_TargetNodeId_TargetTreadId_Success()
        {
            var inputKey = "--input";
            var outputKey = "--output";
            var targetNodeIdKey = "--target-nodeid";
            var targetTreadIdKey = "--target-threadid";

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

            Assert.That(result.Errors.Count, Is.EqualTo(0));

            Assert.That(result.Params.Count, Is.EqualTo(4));

            Assert.That(result.Params.ContainsKey(inputKey), Is.EqualTo(true));
            Assert.That(result.Params[inputKey], Is.EqualTo(@"%USERPROFILE%\SomeInputDir\"));

            Assert.That(result.Params.ContainsKey(outputKey), Is.EqualTo(true));
            Assert.That(result.Params[outputKey], Is.EqualTo(@"%USERPROFILE%\SomeOutputDir\"));

            Assert.That(result.Params.ContainsKey(targetNodeIdKey), Is.EqualTo(true));
            Assert.That(result.Params[targetNodeIdKey], Is.EqualTo("#DummyNPC"));

            Assert.That(result.Params.ContainsKey(targetTreadIdKey), Is.EqualTo(true));
            Assert.That(result.Params[targetTreadIdKey], Is.EqualTo("#020ED339-6313-459A-900D-92F809CEBDC5"));
        }

        [Test]
        public void ValidPositionedCommandLine_SplitByNodes_SplitByThreads_Success()
        {
            var inputKey = "--input";
            var outputKey = "--output";
            var splitByNodesKey = "--split-by-nodes";
            var splitByThreadsKey = "--split-by-threads";

            var args = new List<string>()
                {
                    @"%USERPROFILE%\SomeInputDir\",
                    @"%USERPROFILE%\SomeOutputDir\",
                    "--split-by-nodes",
                    "--split-by-threads"
                };

            var parser = new LogFileBuilderAppCommandLineParser(true);

            var result = parser.Parse(args.ToArray());

            Assert.That(result.Errors.Count, Is.EqualTo(0));

            Assert.That(result.Params.Count, Is.EqualTo(4));

            Assert.That(result.Params.ContainsKey(inputKey), Is.EqualTo(true));
            Assert.That(result.Params[inputKey], Is.EqualTo(@"%USERPROFILE%\SomeInputDir\"));

            Assert.That(result.Params.ContainsKey(outputKey), Is.EqualTo(true));
            Assert.That(result.Params[outputKey], Is.EqualTo(@"%USERPROFILE%\SomeOutputDir\"));

            Assert.That(result.Params.ContainsKey(splitByNodesKey), Is.EqualTo(true));
            Assert.That((result.Params[splitByNodesKey]), Is.EqualTo(true));

            Assert.That(result.Params.ContainsKey(splitByThreadsKey), Is.EqualTo(true));
            Assert.That(result.Params[splitByThreadsKey], Is.EqualTo(true));
        }

        [Test]
        public void ValidNamedCommandLine_SplitByNodes_SplitByThreads_Success()
        {
            var inputKey = "--input";
            var outputKey = "--output";
            var splitByNodesKey = "--split-by-nodes";
            var splitByThreadsKey = "--split-by-threads";

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

            Assert.That(result.Errors.Count, Is.EqualTo(0));

            Assert.That(result.Params.Count, Is.EqualTo(4));

            Assert.That(result.Params.ContainsKey(inputKey), Is.EqualTo(true));
            Assert.That(result.Params[inputKey], Is.EqualTo(@"%USERPROFILE%\SomeInputDir\"));

            Assert.That(result.Params.ContainsKey(outputKey), Is.EqualTo(true));
            Assert.That(result.Params[outputKey], Is.EqualTo(@"%USERPROFILE%\SomeOutputDir\"));

            Assert.That(result.Params.ContainsKey(splitByNodesKey), Is.EqualTo(true));
            Assert.That((result.Params[splitByNodesKey]), Is.EqualTo(true));

            Assert.That(result.Params.ContainsKey(splitByThreadsKey), Is.EqualTo(true));
            Assert.That(result.Params[splitByThreadsKey], Is.EqualTo(true));
        }

        [Test]
        public void ValidPositionedCommandLine_Nologo_Success()
        {
            var inputKey = "--input";
            var outputKey = "--output";
            var nologoKey = "--nologo";

            var args = new List<string>()
                {
                    @"%USERPROFILE%\SomeInputDir\",
                    @"%USERPROFILE%\SomeOutputDir\",
                    "--nologo"
                };

            var parser = new LogFileBuilderAppCommandLineParser(true);

            var result = parser.Parse(args.ToArray());

            Assert.That(result.Errors.Count, Is.EqualTo(0));

            Assert.That(result.Params.Count, Is.EqualTo(3));

            Assert.That(result.Params.ContainsKey(inputKey), Is.EqualTo(true));
            Assert.That(result.Params[inputKey], Is.EqualTo(@"%USERPROFILE%\SomeInputDir\"));

            Assert.That(result.Params.ContainsKey(outputKey), Is.EqualTo(true));
            Assert.That(result.Params[outputKey], Is.EqualTo(@"%USERPROFILE%\SomeOutputDir\"));

            Assert.That(result.Params.ContainsKey(nologoKey), Is.EqualTo(true));
            Assert.That((result.Params[nologoKey]), Is.EqualTo(true));
        }

        [Test]
        public void ValidNamedCommandLine_Nologo_Success()
        {
            var inputKey = "--input";
            var outputKey = "--output";
            var nologoKey = "--nologo";

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

            Assert.That(result.Errors.Count, Is.EqualTo(0));

            Assert.That(result.Params.Count, Is.EqualTo(3));

            Assert.That(result.Params.ContainsKey(inputKey), Is.EqualTo(true));
            Assert.That(result.Params[inputKey], Is.EqualTo(@"%USERPROFILE%\SomeInputDir\"));

            Assert.That(result.Params.ContainsKey(outputKey), Is.EqualTo(true));
            Assert.That(result.Params[outputKey], Is.EqualTo(@"%USERPROFILE%\SomeOutputDir\"));

            Assert.That(result.Params.ContainsKey(nologoKey), Is.EqualTo(true));
            Assert.That((result.Params[nologoKey]), Is.EqualTo(true));
        }

        [Test]
        public void ValidPositionedCommandLine_Configuration_Success()
        {
            var inputKey = "--input";
            var outputKey = "--output";
            var configurationKey = "--configuration";

            var args = new List<string>()
                {
                    @"%USERPROFILE%\SomeInputDir\",
                    @"%USERPROFILE%\SomeOutputDir\",
                    "--configuration",
                    @"%USERPROFILE%\Some.config"
                };

            var parser = new LogFileBuilderAppCommandLineParser(true);

            var result = parser.Parse(args.ToArray());

            Assert.That(result.Errors.Count, Is.EqualTo(0));

            Assert.That(result.Params.Count, Is.EqualTo(3));

            Assert.That(result.Params.ContainsKey(inputKey), Is.EqualTo(true));
            Assert.That(result.Params[inputKey], Is.EqualTo(@"%USERPROFILE%\SomeInputDir\"));

            Assert.That(result.Params.ContainsKey(outputKey), Is.EqualTo(true));
            Assert.That(result.Params[outputKey], Is.EqualTo(@"%USERPROFILE%\SomeOutputDir\"));

            Assert.That(result.Params.ContainsKey(configurationKey), Is.EqualTo(true));
            Assert.That((result.Params[configurationKey]), Is.EqualTo(@"%USERPROFILE%\Some.config"));
        }

        [Test]
        public void ValidNamedCommandLine_Configuration_Success()
        {
            var inputKey = "--input";
            var outputKey = "--output";
            var configurationKey = "--configuration";

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

            Assert.That(result.Errors.Count, Is.EqualTo(0));

            Assert.That(result.Params.Count, Is.EqualTo(3));

            Assert.That(result.Params.ContainsKey(inputKey), Is.EqualTo(true));
            Assert.That(result.Params[inputKey], Is.EqualTo(@"%USERPROFILE%\SomeInputDir\"));

            Assert.That(result.Params.ContainsKey(outputKey), Is.EqualTo(true));
            Assert.That(result.Params[outputKey], Is.EqualTo(@"%USERPROFILE%\SomeOutputDir\"));

            Assert.That(result.Params.ContainsKey(configurationKey), Is.EqualTo(true));
            Assert.That((result.Params[configurationKey]), Is.EqualTo(@"%USERPROFILE%\Some.config"));
        }

        [Test]
        public void ValidPositionedCommandLine_Html_Success()
        {
            var inputKey = "--input";
            var outputKey = "--output";
            var htmlKey = "--html";

            var args = new List<string>()
                {
                    @"%USERPROFILE%\SomeInputDir\",
                    @"%USERPROFILE%\SomeOutputDir\",
                    "--html"
                };

            var parser = new LogFileBuilderAppCommandLineParser(true);

            var result = parser.Parse(args.ToArray());

            Assert.That(result.Errors.Count, Is.EqualTo(0));

            Assert.That(result.Params.Count, Is.EqualTo(3));

            Assert.That(result.Params.ContainsKey(inputKey), Is.EqualTo(true));
            Assert.That(result.Params[inputKey], Is.EqualTo(@"%USERPROFILE%\SomeInputDir\"));

            Assert.That(result.Params.ContainsKey(outputKey), Is.EqualTo(true));
            Assert.That(result.Params[outputKey], Is.EqualTo(@"%USERPROFILE%\SomeOutputDir\"));

            Assert.That(result.Params.ContainsKey(htmlKey), Is.EqualTo(true));
            Assert.That((result.Params[htmlKey]), Is.EqualTo(true));
        }

        [Test]
        public void ValidNamedCommandLine_Html_Success()
        {
            var inputKey = "--input";
            var outputKey = "--output";
            var htmlKey = "--html";

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

            Assert.That(result.Errors.Count, Is.EqualTo(0));

            Assert.That(result.Params.Count, Is.EqualTo(3));

            Assert.That(result.Params.ContainsKey(inputKey), Is.EqualTo(true));
            Assert.That(result.Params[inputKey], Is.EqualTo(@"%USERPROFILE%\SomeInputDir\"));

            Assert.That(result.Params.ContainsKey(outputKey), Is.EqualTo(true));
            Assert.That(result.Params[outputKey], Is.EqualTo(@"%USERPROFILE%\SomeOutputDir\"));

            Assert.That(result.Params.ContainsKey(htmlKey), Is.EqualTo(true));
            Assert.That((result.Params[htmlKey]), Is.EqualTo(true));
        }

        [Test]
        public void ValidPositionedCommandLine_Html_AbsUrl_Success()
        {
            var inputKey = "--input";
            var outputKey = "--output";
            var htmlKey = "--html";
            var absUrlKey = "--abs-url";

            var args = new List<string>()
                {
                    @"%USERPROFILE%\SomeInputDir\",
                    @"%USERPROFILE%\SomeOutputDir\",
                    "--html",
                    "--abs-url"
                };

            var parser = new LogFileBuilderAppCommandLineParser(true);

            var result = parser.Parse(args.ToArray());

            Assert.That(result.Errors.Count, Is.EqualTo(0));

            Assert.That(result.Params.Count, Is.EqualTo(4));

            Assert.That(result.Params.ContainsKey(inputKey), Is.EqualTo(true));
            Assert.That(result.Params[inputKey], Is.EqualTo(@"%USERPROFILE%\SomeInputDir\"));

            Assert.That(result.Params.ContainsKey(outputKey), Is.EqualTo(true));
            Assert.That(result.Params[outputKey], Is.EqualTo(@"%USERPROFILE%\SomeOutputDir\"));

            Assert.That(result.Params.ContainsKey(htmlKey), Is.EqualTo(true));
            Assert.That((result.Params[htmlKey]), Is.EqualTo(true));

            Assert.That(result.Params.ContainsKey(absUrlKey), Is.EqualTo(true));
            Assert.That(result.Params[absUrlKey], Is.EqualTo(true));
        }

        [Test]
        public void ValidNamedCommandLine_Html_AbsUrl_Success()
        {
            var inputKey = "--input";
            var outputKey = "--output";
            var htmlKey = "--html";
            var absUrlKey = "--abs-url";

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

            Assert.That(result.Errors.Count, Is.EqualTo(0));

            Assert.That(result.Params.Count, Is.EqualTo(4));

            Assert.That(result.Params.ContainsKey(inputKey), Is.EqualTo(true));
            Assert.That(result.Params[inputKey], Is.EqualTo(@"%USERPROFILE%\SomeInputDir\"));

            Assert.That(result.Params.ContainsKey(outputKey), Is.EqualTo(true));
            Assert.That(result.Params[outputKey], Is.EqualTo(@"%USERPROFILE%\SomeOutputDir\"));

            Assert.That(result.Params.ContainsKey(htmlKey), Is.EqualTo(true));
            Assert.That((result.Params[htmlKey]), Is.EqualTo(true));

            Assert.That(result.Params.ContainsKey(absUrlKey), Is.EqualTo(true));
            Assert.That(result.Params[absUrlKey], Is.EqualTo(true));
        }

        [Test]
        public void ValidNamedCommandLine_Help_Success()
        {
            var helpKey = "--help";

            var args = new List<string>()
            {
                "--help"
            };

            var parser = new LogFileBuilderAppCommandLineParser(true);

            var result = parser.Parse(args.ToArray());

            Assert.That(result.Errors.Count, Is.EqualTo(0));

            Assert.That(result.Params.Count, Is.EqualTo(1));

            Assert.That(result.Params.ContainsKey(helpKey), Is.EqualTo(true));
            Assert.That((result.Params[helpKey]), Is.EqualTo(true));
        }

        [Test]
        public void FullValidPositionedCommandLine_Success()
        {
            var inputKey = "--input";
            var outputKey = "--output";
            var targetNodeIdKey = "--target-nodeid";
            var targetTreadIdKey = "--target-threadid";
            var splitByNodesKey = "--split-by-nodes";
            var splitByThreadsKey = "--split-by-threads";
            var nologoKey = "--nologo";
            var configurationKey = "--configuration";
            var htmlKey = "--html";
            var absUrlKey = "--abs-url";

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

            Assert.That(result.Errors.Count, Is.EqualTo(0));

            Assert.That(result.Params.Count, Is.EqualTo(10));

            Assert.That(result.Params.ContainsKey(inputKey), Is.EqualTo(true));
            Assert.That(result.Params[inputKey], Is.EqualTo(@"%USERPROFILE%\SomeInputDir\"));

            Assert.That(result.Params.ContainsKey(outputKey), Is.EqualTo(true));
            Assert.That(result.Params[outputKey], Is.EqualTo(@"%USERPROFILE%\SomeOutputDir\"));

            Assert.That(result.Params.ContainsKey(targetNodeIdKey), Is.EqualTo(true));
            Assert.That(result.Params[targetNodeIdKey], Is.EqualTo("#DummyNPC"));

            Assert.That(result.Params.ContainsKey(targetTreadIdKey), Is.EqualTo(true));
            Assert.That(result.Params[targetTreadIdKey], Is.EqualTo("#020ED339-6313-459A-900D-92F809CEBDC5"));

            Assert.That(result.Params.ContainsKey(splitByNodesKey), Is.EqualTo(true));
            Assert.That((result.Params[splitByNodesKey]), Is.EqualTo(true));

            Assert.That(result.Params.ContainsKey(splitByThreadsKey), Is.EqualTo(true));
            Assert.That(result.Params[splitByThreadsKey], Is.EqualTo(true));

            Assert.That(result.Params.ContainsKey(nologoKey), Is.EqualTo(true));
            Assert.That((result.Params[nologoKey]), Is.EqualTo(true));

            Assert.That(result.Params.ContainsKey(configurationKey), Is.EqualTo(true));
            Assert.That((result.Params[configurationKey]), Is.EqualTo(@"%USERPROFILE%\Some.config"));

            Assert.That(result.Params.ContainsKey(htmlKey), Is.EqualTo(true));
            Assert.That((result.Params[htmlKey]), Is.EqualTo(true));

            Assert.That(result.Params.ContainsKey(absUrlKey), Is.EqualTo(true));
            Assert.That(result.Params[absUrlKey], Is.EqualTo(true));
        }

        [Test]
        public void FullValidNamedCommandLine_Success()
        {
            var inputKey = "--input";
            var outputKey = "--output";
            var targetNodeIdKey = "--target-nodeid";
            var targetTreadIdKey = "--target-threadid";
            var splitByNodesKey = "--split-by-nodes";
            var splitByThreadsKey = "--split-by-threads";
            var nologoKey = "--nologo";
            var configurationKey = "--configuration";
            var htmlKey = "--html";
            var absUrlKey = "--abs-url";

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

            Assert.That(result.Errors.Count, Is.EqualTo(0));

            Assert.That(result.Params.Count, Is.EqualTo(10));

            Assert.That(result.Params.ContainsKey(inputKey), Is.EqualTo(true));
            Assert.That(result.Params[inputKey], Is.EqualTo(@"%USERPROFILE%\SomeInputDir\"));

            Assert.That(result.Params.ContainsKey(outputKey), Is.EqualTo(true));
            Assert.That(result.Params[outputKey], Is.EqualTo(@"%USERPROFILE%\SomeOutputDir\"));

            Assert.That(result.Params.ContainsKey(targetNodeIdKey), Is.EqualTo(true));
            Assert.That(result.Params[targetNodeIdKey], Is.EqualTo("#DummyNPC"));

            Assert.That(result.Params.ContainsKey(targetTreadIdKey), Is.EqualTo(true));
            Assert.That(result.Params[targetTreadIdKey], Is.EqualTo("#020ED339-6313-459A-900D-92F809CEBDC5"));

            Assert.That(result.Params.ContainsKey(splitByNodesKey), Is.EqualTo(true));
            Assert.That((result.Params[splitByNodesKey]), Is.EqualTo(true));

            Assert.That(result.Params.ContainsKey(splitByThreadsKey), Is.EqualTo(true));
            Assert.That(result.Params[splitByThreadsKey], Is.EqualTo(true));

            Assert.That(result.Params.ContainsKey(nologoKey), Is.EqualTo(true));
            Assert.That((result.Params[nologoKey]), Is.EqualTo(true));

            Assert.That(result.Params.ContainsKey(configurationKey), Is.EqualTo(true));
            Assert.That((result.Params[configurationKey]), Is.EqualTo(@"%USERPROFILE%\Some.config"));

            Assert.That(result.Params.ContainsKey(htmlKey), Is.EqualTo(true));
            Assert.That((result.Params[htmlKey]), Is.EqualTo(true));

            Assert.That(result.Params.ContainsKey(absUrlKey), Is.EqualTo(true));
            Assert.That(result.Params[absUrlKey], Is.EqualTo(true));
        }

        [Test]
        public void EmptyCommandLine_Success()
        {
            var parser = new LogFileBuilderAppCommandLineParser(true);

            throw new NotImplementedException();
        }

        [Test]
        public void MinimalValidPositionedCommandline_Help_Fail()
        {
            var parser = new LogFileBuilderAppCommandLineParser(false);

            throw new NotImplementedException();
        }

        [Test]
        public void MinimalValidPositionedCommandline_Help_ErrorsList()
        {
            var parser = new LogFileBuilderAppCommandLineParser(true);

            throw new NotImplementedException();
        }

        [Test]
        public void MinimalValidNamedCommandline_Help_Fail()
        {
            var parser = new LogFileBuilderAppCommandLineParser(false);

            throw new NotImplementedException();
        }

        [Test]
        public void MinimalValidNamedCommandline_Help_ErrorsList()
        {
            var parser = new LogFileBuilderAppCommandLineParser(true);

            throw new NotImplementedException();
        }

        [Test]
        public void MinimalValidPositionedCommandline_AbsUrl_Fail()
        {
            var parser = new LogFileBuilderAppCommandLineParser(false);

            throw new NotImplementedException();
        }

        [Test]
        public void MinimalValidPositionedCommandline_AbsUrl_ErrorsList()
        {
            var parser = new LogFileBuilderAppCommandLineParser(true);

            throw new NotImplementedException();
        }

        [Test]
        public void MinimalValidNamedCommandline_AbsUrl_Fail()
        {
            var parser = new LogFileBuilderAppCommandLineParser(false);

            throw new NotImplementedException();
        }

        [Test]
        public void MinimalValidNamedCommandline_AbsUrl_ErrorsList()
        {
            var parser = new LogFileBuilderAppCommandLineParser(true);

            throw new NotImplementedException();
        }
    }
}
