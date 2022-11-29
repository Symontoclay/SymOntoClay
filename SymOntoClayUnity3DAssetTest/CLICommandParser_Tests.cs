/*MIT License

Copyright (c) 2020 - 2022 Sergiy Tolkachov

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

        [Test]
        [Parallelizable]
        public void Case8()
        {
            var commandLine = "new -nav Road1";

            var args = ParseCommandLine(commandLine);

            var command = CLICommandParser.Parse(args);

            Assert.IsNotNull(command);
            Assert.AreEqual(KindOfCLICommand.New, command.Kind);
            Assert.AreEqual(KindOfNewCommand.Nav, command.KindOfNewCommand);
            Assert.AreEqual(null, command.InputFile);
            Assert.AreEqual(null, command.InputDir);
            Assert.AreEqual("Road1", command.ProjectName);
            Assert.AreEqual(false, command.NoLogo);
            Assert.AreEqual(null, command.Timeout);
            Assert.AreEqual(true, command.IsValid);
        }

        [Test]
        [Parallelizable]
        public void Case8_a()
        {
            var commandLine = "n -nav Road1";

            var args = ParseCommandLine(commandLine);

            var command = CLICommandParser.Parse(args);

            Assert.IsNotNull(command);
            Assert.AreEqual(KindOfCLICommand.New, command.Kind);
            Assert.AreEqual(KindOfNewCommand.Nav, command.KindOfNewCommand);
            Assert.AreEqual(null, command.InputFile);
            Assert.AreEqual(null, command.InputDir);
            Assert.AreEqual("Road1", command.ProjectName);
            Assert.AreEqual(false, command.NoLogo);
            Assert.AreEqual(null, command.Timeout);
            Assert.AreEqual(true, command.IsValid);
        }

        [Test]
        [Parallelizable]
        public void Case9()
        {
            var commandLine = "new -lib SomeLib1";

            var args = ParseCommandLine(commandLine);

            var command = CLICommandParser.Parse(args);

            Assert.IsNotNull(command);
            Assert.AreEqual(KindOfCLICommand.New, command.Kind);
            Assert.AreEqual(KindOfNewCommand.Lib, command.KindOfNewCommand);
            Assert.AreEqual(null, command.InputFile);
            Assert.AreEqual(null, command.InputDir);
            Assert.AreEqual("SomeLib1", command.ProjectName);
            Assert.AreEqual(false, command.NoLogo);
            Assert.AreEqual(null, command.Timeout);
            Assert.AreEqual(true, command.IsValid);
        }

        [Test]
        [Parallelizable]
        public void Case9_b()
        {
            var commandLine = "new -l SomeLib1";

            var args = ParseCommandLine(commandLine);

            var command = CLICommandParser.Parse(args);

            Assert.IsNotNull(command);
            Assert.AreEqual(KindOfCLICommand.New, command.Kind);
            Assert.AreEqual(KindOfNewCommand.Lib, command.KindOfNewCommand);
            Assert.AreEqual(null, command.InputFile);
            Assert.AreEqual(null, command.InputDir);
            Assert.AreEqual("SomeLib1", command.ProjectName);
            Assert.AreEqual(false, command.NoLogo);
            Assert.AreEqual(null, command.Timeout);
            Assert.AreEqual(true, command.IsValid);
        }

        [Test]
        [Parallelizable]
        public void Case9_c()
        {
            var commandLine = "n -lib SomeLib1";

            var args = ParseCommandLine(commandLine);

            var command = CLICommandParser.Parse(args);

            Assert.IsNotNull(command);
            Assert.AreEqual(KindOfCLICommand.New, command.Kind);
            Assert.AreEqual(KindOfNewCommand.Lib, command.KindOfNewCommand);
            Assert.AreEqual(null, command.InputFile);
            Assert.AreEqual(null, command.InputDir);
            Assert.AreEqual("SomeLib1", command.ProjectName);
            Assert.AreEqual(false, command.NoLogo);
            Assert.AreEqual(null, command.Timeout);
            Assert.AreEqual(true, command.IsValid);
        }

        [Test]
        [Parallelizable]
        public void Case9_d()
        {
            var commandLine = "n -l SomeLib1";

            var args = ParseCommandLine(commandLine);

            var command = CLICommandParser.Parse(args);

            Assert.IsNotNull(command);
            Assert.AreEqual(KindOfCLICommand.New, command.Kind);
            Assert.AreEqual(KindOfNewCommand.Lib, command.KindOfNewCommand);
            Assert.AreEqual(null, command.InputFile);
            Assert.AreEqual(null, command.InputDir);
            Assert.AreEqual("SomeLib1", command.ProjectName);
            Assert.AreEqual(false, command.NoLogo);
            Assert.AreEqual(null, command.Timeout);
            Assert.AreEqual(true, command.IsValid);
        }

        [Test]
        [Parallelizable]
        public void Case10()
        {
            var commandLine = "new -player Player1";

            var args = ParseCommandLine(commandLine);

            var command = CLICommandParser.Parse(args);

            Assert.IsNotNull(command);
            Assert.AreEqual(KindOfCLICommand.New, command.Kind);
            Assert.AreEqual(KindOfNewCommand.Player, command.KindOfNewCommand);
            Assert.AreEqual(null, command.InputFile);
            Assert.AreEqual(null, command.InputDir);
            Assert.AreEqual("Player1", command.ProjectName);
            Assert.AreEqual(false, command.NoLogo);
            Assert.AreEqual(null, command.Timeout);
            Assert.AreEqual(true, command.IsValid);
        }

        [Test]
        [Parallelizable]
        public void Case10_a()
        {
            var commandLine = "new -p Player1";

            var args = ParseCommandLine(commandLine);

            var command = CLICommandParser.Parse(args);

            Assert.IsNotNull(command);
            Assert.AreEqual(KindOfCLICommand.New, command.Kind);
            Assert.AreEqual(KindOfNewCommand.Player, command.KindOfNewCommand);
            Assert.AreEqual(null, command.InputFile);
            Assert.AreEqual(null, command.InputDir);
            Assert.AreEqual("Player1", command.ProjectName);
            Assert.AreEqual(false, command.NoLogo);
            Assert.AreEqual(null, command.Timeout);
            Assert.AreEqual(true, command.IsValid);
        }

        [Test]
        [Parallelizable]
        public void Case10_b()
        {
            var commandLine = "n -player Player1";

            var args = ParseCommandLine(commandLine);

            var command = CLICommandParser.Parse(args);

            Assert.IsNotNull(command);
            Assert.AreEqual(KindOfCLICommand.New, command.Kind);
            Assert.AreEqual(KindOfNewCommand.Player, command.KindOfNewCommand);
            Assert.AreEqual(null, command.InputFile);
            Assert.AreEqual(null, command.InputDir);
            Assert.AreEqual("Player1", command.ProjectName);
            Assert.AreEqual(false, command.NoLogo);
            Assert.AreEqual(null, command.Timeout);
            Assert.AreEqual(true, command.IsValid);
        }

        [Test]
        [Parallelizable]
        public void Case10_c()
        {
            var commandLine = "n -p Player1";

            var args = ParseCommandLine(commandLine);

            var command = CLICommandParser.Parse(args);

            Assert.IsNotNull(command);
            Assert.AreEqual(KindOfCLICommand.New, command.Kind);
            Assert.AreEqual(KindOfNewCommand.Player, command.KindOfNewCommand);
            Assert.AreEqual(null, command.InputFile);
            Assert.AreEqual(null, command.InputDir);
            Assert.AreEqual("Player1", command.ProjectName);
            Assert.AreEqual(false, command.NoLogo);
            Assert.AreEqual(null, command.Timeout);
            Assert.AreEqual(true, command.IsValid);
        }

        [Test]
        [Parallelizable]
        public void Case11()
        {
            var commandLine = "version";

            var args = ParseCommandLine(commandLine);

            var command = CLICommandParser.Parse(args);

            Assert.IsNotNull(command);
            Assert.AreEqual(KindOfCLICommand.Version, command.Kind);
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
        public void Case11_a()
        {
            var commandLine = "v";

            var args = ParseCommandLine(commandLine);

            var command = CLICommandParser.Parse(args);

            Assert.IsNotNull(command);
            Assert.AreEqual(KindOfCLICommand.Version, command.Kind);
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
        public void Case11_b()
        {
            var commandLine = "version -nologo -timeout 10000";

            var args = ParseCommandLine(commandLine);

            var command = CLICommandParser.Parse(args);

            Assert.IsNotNull(command);
            Assert.AreEqual(KindOfCLICommand.Version, command.Kind);
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
        public void Case11_c()
        {
            var commandLine = "v -nologo -timeout 10000";

            var args = ParseCommandLine(commandLine);

            var command = CLICommandParser.Parse(args);

            Assert.IsNotNull(command);
            Assert.AreEqual(KindOfCLICommand.Version, command.Kind);
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
        public void Case12()
        {
            var commandLine = "install stdlib";

            var args = ParseCommandLine(commandLine);

            var command = CLICommandParser.Parse(args);

            Assert.IsNotNull(command);
            Assert.AreEqual(KindOfCLICommand.Install, command.Kind);
            Assert.AreEqual(KindOfNewCommand.Unknown, command.KindOfNewCommand);
            Assert.AreEqual(null, command.InputFile);
            Assert.AreEqual(true, !string.IsNullOrWhiteSpace(command.InputDir));
            Assert.AreEqual("stdlib", command.ProjectName);
            Assert.AreEqual(false, command.NoLogo);
            Assert.AreEqual(null, command.Timeout);
            Assert.AreEqual(true, command.IsValid);
        }

        [Test]
        [Parallelizable]
        public void Case12_a()
        {
            var commandLine = "install stdlib -nologo";

            var args = ParseCommandLine(commandLine);

            var command = CLICommandParser.Parse(args);

            Assert.IsNotNull(command);
            Assert.AreEqual(KindOfCLICommand.Install, command.Kind);
            Assert.AreEqual(KindOfNewCommand.Unknown, command.KindOfNewCommand);
            Assert.AreEqual(null, command.InputFile);
            Assert.AreEqual(true, !string.IsNullOrWhiteSpace(command.InputDir));
            Assert.AreEqual("stdlib", command.ProjectName);
            Assert.AreEqual(true, command.NoLogo);
            Assert.AreEqual(null, command.Timeout);
            Assert.AreEqual(true, command.IsValid);
        }

        private string[] ParseCommandLine(string value)
        {
            return value.Split(' ');
        }
    }
}
