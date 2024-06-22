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
            Assert.That(((string)result.Params[inputKey]), Is.EqualTo(@"%USERPROFILE%\SomeInputDir\"));

            Assert.That(result.Params.ContainsKey(outputKey), Is.EqualTo(true));
            Assert.That(((string)result.Params[outputKey]), Is.EqualTo(@"%USERPROFILE%\SomeOutputDir\"));
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
            Assert.That(((string)result.Params[inputKey]), Is.EqualTo(@"%USERPROFILE%\SomeInputDir\"));

            Assert.That(result.Params.ContainsKey(outputKey), Is.EqualTo(true));
            Assert.That(((string)result.Params[outputKey]), Is.EqualTo(@"%USERPROFILE%\SomeOutputDir\"));
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
            Assert.That(((string)result.Params[inputKey]), Is.EqualTo(@"%USERPROFILE%\SomeInputDir\"));

            Assert.That(result.Params.ContainsKey(outputKey), Is.EqualTo(true));
            Assert.That(((string)result.Params[outputKey]), Is.EqualTo(@"%USERPROFILE%\SomeOutputDir\"));

            Assert.That(result.Params.ContainsKey(targetNodeIdKey), Is.EqualTo(true));
            Assert.That(((string)result.Params[targetNodeIdKey]), Is.EqualTo("#DummyNPC"));

            Assert.That(result.Params.ContainsKey(targetTreadIdKey), Is.EqualTo(true));
            Assert.That(((string)result.Params[targetTreadIdKey]), Is.EqualTo("#020ED339-6313-459A-900D-92F809CEBDC5"));
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
            Assert.That(((string)result.Params[inputKey]), Is.EqualTo(@"%USERPROFILE%\SomeInputDir\"));

            Assert.That(result.Params.ContainsKey(outputKey), Is.EqualTo(true));
            Assert.That(((string)result.Params[outputKey]), Is.EqualTo(@"%USERPROFILE%\SomeOutputDir\"));

            Assert.That(result.Params.ContainsKey(targetNodeIdKey), Is.EqualTo(true));
            Assert.That(((string)result.Params[targetNodeIdKey]), Is.EqualTo("#DummyNPC"));

            Assert.That(result.Params.ContainsKey(targetTreadIdKey), Is.EqualTo(true));
            Assert.That(((string)result.Params[targetTreadIdKey]), Is.EqualTo("#020ED339-6313-459A-900D-92F809CEBDC5"));
        }

        [Test]
        public void ValidPositionedCommandLine_SplitByNodes_SplitByThreads_Success()
        {
            var parser = new LogFileBuilderAppCommandLineParser(true);

            throw new NotImplementedException();
        }

        [Test]
        public void ValidNamedCommandLine_SplitByNodes_SplitByThreads_Success()
        {
            var parser = new LogFileBuilderAppCommandLineParser(true);

            throw new NotImplementedException();
        }

        [Test]
        public void ValidPositionedCommandLine_Nologo_Success()
        {
            var parser = new LogFileBuilderAppCommandLineParser(true);

            throw new NotImplementedException();
        }

        [Test]
        public void ValidNamedCommandLine_Nologo_Success()
        {
            var parser = new LogFileBuilderAppCommandLineParser(true);

            throw new NotImplementedException();
        }

        [Test]
        public void ValidPositionedCommandLine_Configuration_Success()
        {
            var parser = new LogFileBuilderAppCommandLineParser(true);

            throw new NotImplementedException();
        }

        [Test]
        public void ValidNamedCommandLine_Configuration_Success()
        {
            var parser = new LogFileBuilderAppCommandLineParser(true);

            throw new NotImplementedException();
        }

        [Test]
        public void ValidPositionedCommandLine_Html_Success()
        {
            var parser = new LogFileBuilderAppCommandLineParser(true);

            throw new NotImplementedException();
        }

        [Test]
        public void ValidNamedCommandLine_Html_Success()
        {
            var parser = new LogFileBuilderAppCommandLineParser(true);

            throw new NotImplementedException();
        }

        [Test]
        public void ValidPositionedCommandLine_Html_AbsUrl_Success()
        {
            var parser = new LogFileBuilderAppCommandLineParser(true);

            throw new NotImplementedException();
        }

        [Test]
        public void ValidNamedCommandLine_Html_AbsUrl_Success()
        {
            var parser = new LogFileBuilderAppCommandLineParser(true);

            throw new NotImplementedException();
        }

        [Test]
        public void ValidPositionedCommandLine_Help_Success()
        {
            var parser = new LogFileBuilderAppCommandLineParser(true);

            throw new NotImplementedException();
        }

        [Test]
        public void ValidNamedCommandLine_Help_Success()
        {
            var parser = new LogFileBuilderAppCommandLineParser(true);

            throw new NotImplementedException();
        }

        [Test]
        public void FullValidPositionedCommandLine_Success()
        {
            var parser = new LogFileBuilderAppCommandLineParser(true);

            throw new NotImplementedException();
        }

        [Test]
        public void FullValidNamedCommandLine_Success()
        {
            var parser = new LogFileBuilderAppCommandLineParser(true);

            throw new NotImplementedException();
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
