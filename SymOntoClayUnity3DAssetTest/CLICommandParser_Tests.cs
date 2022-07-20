using NUnit.Framework;
using SymOntoClay.CLI;
using SymOntoClayProjectFiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.UnityAsset.Core.Tests
{
    public class CLICommandParser_Tests
    {
        [Test]
        [Parallelizable]
        public void Case1()
        {
            var commandLine = "help";

            var args = ParseCommandLine(commandLine);

            var command = CLICommandParser.Parse(args);

            Assert.IsNotNull(command);
            Assert.AreEqual(KindOfCLICommand.Help, command.Kind);
            Assert.AreEqual(KindOfNewCommand.Unknown, command.KindOfNewCommand);
            Assert.AreEqual(null, command.InputFile);
            Assert.AreEqual(null, command.InputDir);
            Assert.AreEqual(null, command.ProjectName);
            Assert.AreEqual(false, command.NoLogo);
            Assert.AreEqual(null, command.Timeout);
            Assert.AreEqual(true, command.IsValid);
        }

        [Test]
        [Parallelizable]
        public void Case1_a()
        {
            var commandLine = "h";

            var args = ParseCommandLine(commandLine);

            var command = CLICommandParser.Parse(args);

            Assert.IsNotNull(command);
            Assert.AreEqual(KindOfCLICommand.Help, command.Kind);
            Assert.AreEqual(KindOfNewCommand.Unknown, command.KindOfNewCommand);
            Assert.AreEqual(null, command.InputFile);
            Assert.AreEqual(null, command.InputDir);
            Assert.AreEqual(null, command.ProjectName);
            Assert.AreEqual(false, command.NoLogo);
            Assert.AreEqual(null, command.Timeout);
            Assert.AreEqual(true, command.IsValid);
        }

        [Test]
        [Parallelizable]
        public void Case1_b()
        {
            var commandLine = "help -nologo -timeout 10000";

            var args = ParseCommandLine(commandLine);

            var command = CLICommandParser.Parse(args);

            Assert.IsNotNull(command);
            Assert.AreEqual(KindOfCLICommand.Help, command.Kind);
            Assert.AreEqual(KindOfNewCommand.Unknown, command.KindOfNewCommand);
            Assert.AreEqual(null, command.InputFile);
            Assert.AreEqual(null, command.InputDir);
            Assert.AreEqual(null, command.ProjectName);
            Assert.AreEqual(true, command.NoLogo);
            Assert.AreEqual(10000, command.Timeout);
            Assert.AreEqual(true, command.IsValid);
        }

        [Test]
        [Parallelizable]
        public void Case1_c()
        {
            var commandLine = "h -nologo -timeout 10000";

            var args = ParseCommandLine(commandLine);

            var command = CLICommandParser.Parse(args);

            Assert.IsNotNull(command);
            Assert.AreEqual(KindOfCLICommand.Help, command.Kind);
            Assert.AreEqual(KindOfNewCommand.Unknown, command.KindOfNewCommand);
            Assert.AreEqual(null, command.InputFile);
            Assert.AreEqual(null, command.InputDir);
            Assert.AreEqual(null, command.ProjectName);
            Assert.AreEqual(true, command.NoLogo);
            Assert.AreEqual(10000, command.Timeout);
            Assert.AreEqual(true, command.IsValid);
        }

        [Test]
        [Parallelizable]
        public void Case2()
        {
            var commandLine = "run PeaceKeeper";

            var args = ParseCommandLine(commandLine);

            var command = CLICommandParser.Parse(args);

            Assert.IsNotNull(command);
            Assert.AreEqual(KindOfCLICommand.Run, command.Kind);
            Assert.AreEqual(KindOfNewCommand.Unknown, command.KindOfNewCommand);
            Assert.AreEqual(true, command.InputFile.EndsWith(@"\PeaceKeeper"));
            Assert.AreEqual(null, command.InputDir);
            Assert.AreEqual(null, command.ProjectName);
            Assert.AreEqual(false, command.NoLogo);
            Assert.AreEqual(null, command.Timeout);
            Assert.AreEqual(true, command.IsValid);
        }

        [Test]
        [Parallelizable]
        public void Case2_a()
        {
            var commandLine = "run PeaceKeeper -nologo -timeout 10000";

            var args = ParseCommandLine(commandLine);

            var command = CLICommandParser.Parse(args);

            Assert.IsNotNull(command);
            Assert.AreEqual(KindOfCLICommand.Run, command.Kind);
            Assert.AreEqual(KindOfNewCommand.Unknown, command.KindOfNewCommand);
            Assert.AreEqual(true, command.InputFile.EndsWith(@"\PeaceKeeper"));
            Assert.AreEqual(null, command.InputDir);
            Assert.AreEqual(null, command.ProjectName);
            Assert.AreEqual(true, command.NoLogo);
            Assert.AreEqual(10000, command.Timeout);
            Assert.AreEqual(true, command.IsValid);
        }

        [Test]
        [Parallelizable]
        public void Case3()
        {
            var commandLine = "run";

            var args = ParseCommandLine(commandLine);

            var command = CLICommandParser.Parse(args);

            Assert.IsNotNull(command);
            Assert.AreEqual(KindOfCLICommand.Run, command.Kind);
            Assert.AreEqual(KindOfNewCommand.Unknown, command.KindOfNewCommand);
            Assert.AreEqual(null, command.InputFile);
            Assert.AreEqual(true, command.InputDir.Length > 0);
            Assert.AreEqual(null, command.ProjectName);
            Assert.AreEqual(false, command.NoLogo);
            Assert.AreEqual(null, command.Timeout);
            Assert.AreEqual(true, command.IsValid);
        }

        [Test]
        [Parallelizable]
        public void Case3_a()
        {
            var commandLine = "run -nologo -timeout 10000";

            var args = ParseCommandLine(commandLine);

            var command = CLICommandParser.Parse(args);

            Assert.IsNotNull(command);
            Assert.AreEqual(KindOfCLICommand.Run, command.Kind);
            Assert.AreEqual(KindOfNewCommand.Unknown, command.KindOfNewCommand);
            Assert.AreEqual(null, command.InputFile);
            Assert.AreEqual(true, command.InputDir.Length > 0);
            Assert.AreEqual(null, command.ProjectName);
            Assert.AreEqual(true, command.NoLogo);
            Assert.AreEqual(10000, command.Timeout);
            Assert.AreEqual(true, command.IsValid);
        }

        [Test]
        [Parallelizable]
        public void Case4()
        {
            var exception = Assert.Catch<ArgumentOutOfRangeException>(() => {
                var commandLine = "exit";

                var args = ParseCommandLine(commandLine);

                var command = CLICommandParser.Parse(args);
            });

            Assert.AreEqual("firstArg", exception.ParamName);
        }

        [Test]
        [Parallelizable]
        public void Case5()
        {
            var commandLine = "new PeaceKeeper";

            var args = ParseCommandLine(commandLine);

            var command = CLICommandParser.Parse(args);

            Assert.IsNotNull(command);
            Assert.AreEqual(KindOfCLICommand.New, command.Kind);
            Assert.AreEqual(KindOfNewCommand.NPC, command.KindOfNewCommand);
            Assert.AreEqual(null, command.InputFile);
            Assert.AreEqual(null, command.InputDir);
            Assert.AreEqual("PeaceKeeper", command.ProjectName);
            Assert.AreEqual(false, command.NoLogo);
            Assert.AreEqual(null, command.Timeout);
            Assert.AreEqual(true, command.IsValid);
        }

        [Test]
        [Parallelizable]
        public void Case5_a()
        {
            var commandLine = "n PeaceKeeper";

            var args = ParseCommandLine(commandLine);

            var command = CLICommandParser.Parse(args);

            Assert.IsNotNull(command);
            Assert.AreEqual(KindOfCLICommand.New, command.Kind);
            Assert.AreEqual(KindOfNewCommand.NPC, command.KindOfNewCommand);
            Assert.AreEqual(null, command.InputFile);
            Assert.AreEqual(null, command.InputDir);
            Assert.AreEqual("PeaceKeeper", command.ProjectName);
            Assert.AreEqual(false, command.NoLogo);
            Assert.AreEqual(null, command.Timeout);
            Assert.AreEqual(true, command.IsValid);
        }

        [Test]
        [Parallelizable]
        public void Case5_b()
        {
            var commandLine = "new -npc PeaceKeeper";

            var args = ParseCommandLine(commandLine);

            var command = CLICommandParser.Parse(args);

            Assert.IsNotNull(command);
            Assert.AreEqual(KindOfCLICommand.New, command.Kind);
            Assert.AreEqual(KindOfNewCommand.NPC, command.KindOfNewCommand);
            Assert.AreEqual(null, command.InputFile);
            Assert.AreEqual(null, command.InputDir);
            Assert.AreEqual("PeaceKeeper", command.ProjectName);
            Assert.AreEqual(false, command.NoLogo);
            Assert.AreEqual(null, command.Timeout);
            Assert.AreEqual(true, command.IsValid);
        }

        [Test]
        [Parallelizable]
        public void Case5_c()
        {
            var commandLine = "n -npc PeaceKeeper";

            var args = ParseCommandLine(commandLine);

            var command = CLICommandParser.Parse(args);

            Assert.IsNotNull(command);
            Assert.AreEqual(KindOfCLICommand.New, command.Kind);
            Assert.AreEqual(KindOfNewCommand.NPC, command.KindOfNewCommand);
            Assert.AreEqual(null, command.InputFile);
            Assert.AreEqual(null, command.InputDir);
            Assert.AreEqual("PeaceKeeper", command.ProjectName);
            Assert.AreEqual(false, command.NoLogo);
            Assert.AreEqual(null, command.Timeout);
            Assert.AreEqual(true, command.IsValid);
        }

        [Test]
        [Parallelizable]
        public void Case6()
        {
            var commandLine = "new -world Avalon";

            var args = ParseCommandLine(commandLine);

            var command = CLICommandParser.Parse(args);

            Assert.IsNotNull(command);
            Assert.AreEqual(KindOfCLICommand.NewWorlspace, command.Kind);
            Assert.AreEqual(KindOfNewCommand.Unknown, command.KindOfNewCommand);
            Assert.AreEqual(null, command.InputFile);
            Assert.AreEqual(null, command.InputDir);
            Assert.AreEqual("Avalon", command.ProjectName);
            Assert.AreEqual(false, command.NoLogo);
            Assert.AreEqual(null, command.Timeout);
            Assert.AreEqual(true, command.IsValid);
        }

        [Test]
        [Parallelizable]
        public void Case6_a()
        {
            var commandLine = "n -world Avalon";

            var args = ParseCommandLine(commandLine);

            var command = CLICommandParser.Parse(args);

            Assert.IsNotNull(command);
            Assert.AreEqual(KindOfCLICommand.NewWorlspace, command.Kind);
            Assert.AreEqual(KindOfNewCommand.Unknown, command.KindOfNewCommand);
            Assert.AreEqual(null, command.InputFile);
            Assert.AreEqual(null, command.InputDir);
            Assert.AreEqual("Avalon", command.ProjectName);
            Assert.AreEqual(false, command.NoLogo);
            Assert.AreEqual(null, command.Timeout);
            Assert.AreEqual(true, command.IsValid);
        }

        [Test]
        [Parallelizable]
        public void Case6_b()
        {
            var commandLine = "new -w Avalon";

            var args = ParseCommandLine(commandLine);

            var command = CLICommandParser.Parse(args);

            Assert.IsNotNull(command);
            Assert.AreEqual(KindOfCLICommand.NewWorlspace, command.Kind);
            Assert.AreEqual(KindOfNewCommand.Unknown, command.KindOfNewCommand);
            Assert.AreEqual(null, command.InputFile);
            Assert.AreEqual(null, command.InputDir);
            Assert.AreEqual("Avalon", command.ProjectName);
            Assert.AreEqual(false, command.NoLogo);
            Assert.AreEqual(null, command.Timeout);
            Assert.AreEqual(true, command.IsValid);
        }

        [Test]
        [Parallelizable]
        public void Case6_c()
        {
            var commandLine = "n -w Avalon";

            var args = ParseCommandLine(commandLine);

            var command = CLICommandParser.Parse(args);

            Assert.IsNotNull(command);
            Assert.AreEqual(KindOfCLICommand.NewWorlspace, command.Kind);
            Assert.AreEqual(KindOfNewCommand.Unknown, command.KindOfNewCommand);
            Assert.AreEqual(null, command.InputFile);
            Assert.AreEqual(null, command.InputDir);
            Assert.AreEqual("Avalon", command.ProjectName);
            Assert.AreEqual(false, command.NoLogo);
            Assert.AreEqual(null, command.Timeout);
            Assert.AreEqual(true, command.IsValid);
        }

        [Test]
        [Parallelizable]
        public void Case7()
        {
            var commandLine = "new -thing Barrel";

            var args = ParseCommandLine(commandLine);

            var command = CLICommandParser.Parse(args);

            Assert.IsNotNull(command);
            Assert.AreEqual(KindOfCLICommand.New, command.Kind);
            Assert.AreEqual(KindOfNewCommand.Thing, command.KindOfNewCommand);
            Assert.AreEqual(null, command.InputFile);
            Assert.AreEqual(null, command.InputDir);
            Assert.AreEqual("Barrel", command.ProjectName);
            Assert.AreEqual(false, command.NoLogo);
            Assert.AreEqual(null, command.Timeout);
            Assert.AreEqual(true, command.IsValid);
        }

        [Test]
        [Parallelizable]
        public void Case7_a()
        {
            var commandLine = "n -thing Barrel";

            var args = ParseCommandLine(commandLine);

            var command = CLICommandParser.Parse(args);

            Assert.IsNotNull(command);
            Assert.AreEqual(KindOfCLICommand.New, command.Kind);
            Assert.AreEqual(KindOfNewCommand.Thing, command.KindOfNewCommand);
            Assert.AreEqual(null, command.InputFile);
            Assert.AreEqual(null, command.InputDir);
            Assert.AreEqual("Barrel", command.ProjectName);
            Assert.AreEqual(false, command.NoLogo);
            Assert.AreEqual(null, command.Timeout);
            Assert.AreEqual(true, command.IsValid);
        }

        /*
        
         */

        /*
            var commandLine = "";

            var args = ParseCommandLine(commandLine);

            var command = CLICommandParser.Parse(args);

            Assert.IsNotNull(command);
            Assert.AreEqual(KindOfCLICommand., command.Kind);
            Assert.AreEqual(KindOfNewCommand., command.KindOfNewCommand);
            Assert.AreEqual(, command.InputFile);
            Assert.AreEqual(, command.InputDir);
            Assert.AreEqual(, command.ProjectName);
            Assert.AreEqual(, command.NoLogo);
            Assert.AreEqual(, command.Timeout);
            Assert.AreEqual(, command.IsValid);
        */

        /*
            Assert.AreEqual(true, command.NoLogo);
            Assert.AreEqual(10000, command.Timeout);
        */

        private string[] ParseCommandLine(string value)
        {
            return value.Split(' ');
        }
    }
}
