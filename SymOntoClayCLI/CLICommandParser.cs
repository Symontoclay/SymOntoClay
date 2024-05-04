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

using SymOntoClay.Common;
using SymOntoClay.ProjectFiles;
using System;
using System.IO;
using System.Linq;

namespace SymOntoClay.CLI
{
    public static class CLICommandParser
    {
        public static CLICommand Parse(string[] args)
        {
            var argsList = args.ToList();
            
            var command = new CLICommand();

            while(argsList.Any())
            {
                var firstArg = argsList.First();

                var reallyUsedArgs = 0;

                switch (firstArg)
                {
                    case "h":
                    case "help":
                        CheckCommandAsUnknown(command);
                        command.Kind = KindOfCLICommand.Help;
                        command.IsValid = true;
                        reallyUsedArgs = 1;
                        break;

                    case "version":
                    case "v":
                        CheckCommandAsUnknown(command);
                        command.Kind = KindOfCLICommand.Version;
                        command.IsValid = true;
                        reallyUsedArgs = 1;
                        break;

                    case "run":
                        {
                            CheckCommandAsUnknown(command);
                            command.Kind = KindOfCLICommand.Run;

                            var inputFile = string.Empty;

                            if(argsList.Count > 1)
                            {
                                var secondArg = argsList[1];

                                if(!IsAdditionalOptions(secondArg))
                                {
                                    inputFile = secondArg;
                                }                                
                            }

                            if(string.IsNullOrWhiteSpace(inputFile))
                            {
                                command.InputDir = Directory.GetCurrentDirectory();
                                command.IsValid = true;
                                reallyUsedArgs = 1;
                            }
                            else
                            {
                                command.InputFile = EVPath.Normalize(inputFile);
                                command.IsValid = true;
                                reallyUsedArgs = 2;
                            }
                        }
                        break;

                    case "new":
                    case "n":
                        {
                            CheckCommandAsUnknown(command);

                            if(argsList.Count == 1)
                            {
                                throw new Exception(NewPureArgs);
                            }

                            command.Kind = KindOfCLICommand.New;

                            var secondArg = argsList[1];

                            if (IsAdditionalOptions(secondArg))
                            {
                                throw new Exception(NewPureArgs);
                            }

                            if(IsNewCommandClarification(secondArg))
                            {
                                if(argsList.Count == 2)
                                {
                                    throw new Exception(NewAbsenceProjectName);
                                }
                                else
                                {
                                    var thirdArg = argsList[2];

                                    if(IsNewCommandClarification(thirdArg) || IsAdditionalOptions(thirdArg))
                                    {
                                        throw new Exception(NewAbsenceProjectName);
                                    }
                                    else
                                    {
                                        switch(secondArg)
                                        {
                                            case "-w":
                                            case "-world":
                                                command.Kind = KindOfCLICommand.NewWorlspace;
                                                break;

                                            case "-npc":
                                                command.KindOfNewCommand = KindOfNewCommand.NPC;
                                                break;

                                            case "-player":
                                            case "-p":
                                                command.KindOfNewCommand = KindOfNewCommand.Player;
                                                break;

                                            case "-thing":
                                                command.KindOfNewCommand = KindOfNewCommand.Thing;
                                                break;

                                            case "-nav":
                                                command.KindOfNewCommand = KindOfNewCommand.Nav;
                                                break;

                                            case "-lib":
                                            case "-l":
                                                command.KindOfNewCommand = KindOfNewCommand.Lib;
                                                break;

                                            default:
                                                throw new ArgumentOutOfRangeException(nameof(secondArg), secondArg, null);
                                        }

                                        command.ProjectName = args[2];
                                        command.IsValid = true;
                                        reallyUsedArgs = 3;
                                    }
                                }
                            }
                            else
                            {
                                command.KindOfNewCommand = KindOfNewCommand.NPC;
                                command.ProjectName = secondArg;
                                command.IsValid = true;
                                reallyUsedArgs = 2;
                            }
                        }
                        break;

                    case "-nologo":
                        command.NoLogo = true;
                        reallyUsedArgs = 1;
                        break;

                    case "-timeout":
                        {
                            if(argsList.Count == 1)
                            {
                                throw new Exception(TimeoutAbsenceValue);
                            }
                            else
                            {
                                var secondArg = argsList[1];

                                if(int.TryParse(secondArg, out int timeout))
                                {
                                    command.Timeout = timeout;
                                    reallyUsedArgs = 2;
                                }
                                else
                                {
                                    throw new Exception($"Timiout value '{secondArg}' can not be converted to milliseconds!");
                                }
                            }                            
                        }
                        break;

                    case "-nlp":
                        command.UseNLP = true;
                        reallyUsedArgs = 1;
                        break;

                    case "install":
                        {
                            if(argsList.Count == 1)
                            {
                                throw new Exception(LibNameAbsence);
                            }
                            else
                            {
                                var secondArg = argsList[1];

                                CheckCommandAsUnknown(command);

                                command.Kind = KindOfCLICommand.Install;
                                command.IsValid = true;
                                command.ProjectName = secondArg;
                                command.InputDir = Directory.GetCurrentDirectory();
                                reallyUsedArgs = 2;
                            }
                        }
                        break;
                    
                    default:
                        throw new ArgumentOutOfRangeException(nameof(firstArg), firstArg, null);
                }

                argsList = argsList.Skip(reallyUsedArgs).ToList();
            }

            return command;
        }

        private const string NewPureArgs = $"It is not enough arguments. You should put at least Project name!";
        private const string NewAbsenceProjectName = "ProjectName is absent!";
        private const string TimeoutAbsenceValue = "You have used `-timeout` but have not defined the timeout's value in milliseconds!";
        private const string LibNameAbsence = "You have used `install` but have not defined the name of target library!";

        private static bool IsAdditionalOptions(string arg)
        {
            switch(arg)
            {
                case "-nologo":
                case "-timeout":
                case "-nlp":
                    return true;

                default:
                    return false;
            }
        }

        private static bool IsNewCommandClarification(string arg)
        {
            switch (arg)
            {
                case "-w":
                case "-world":
                case "-npc":
                case "-player":
                case "-p":
                case "-thing":
                case "-nav":
                case "-lib":
                case "-l":
                    return true;

                default:
                    return false;
            }
        }

        private static void CheckCommandAsUnknown(CLICommand command)
        {
            if(command.Kind != KindOfCLICommand.Unknown)
            {
                throw new Exception("Mixed modes!");
            }
        }
    }
}
