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

using Newtonsoft.Json;
using NLog;
using SymOntoClay.CoreHelper;
using SymOntoClayProjectFiles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SymOntoClay.CLI
{
    public static class CLICommandParser
    {
#if DEBUG
        //private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
#endif

        public static CLICommand Parse(string[] args)
        {
#if DEBUG
            //_logger.Info($"args = {JsonConvert.SerializeObject(args, Formatting.Indented)}");
#endif

            var argsList = args.ToList();

            var command = new CLICommand();

            while(argsList.Any())
            {
#if DEBUG
                //_logger.Info($"argsList.Count = {argsList.Count}");
#endif

                var firstArg = argsList.First();

#if DEBUG
                //_logger.Info($"firstArg = {firstArg}");
#endif

#if DEBUG
                //_logger.Info($"command = {command}");
#endif

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

#if DEBUG
                            //_logger.Info($"argsList.Count = {argsList.Count}");
#endif

                            if(argsList.Count > 1)
                            {
                                var secondArg = argsList[1];

#if DEBUG
                                //_logger.Info($"secondArg = {secondArg}");
#endif
                                if(!IsAdditionalOptions(secondArg))
                                {
                                    inputFile = secondArg;
                                }                                
                            }

#if DEBUG
                            //_logger.Info($"inputFile = {inputFile}");
#endif

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

#if DEBUG
                            //_logger.Info($"secondArg = {secondArg}");
#endif

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

#if DEBUG
                                    //_logger.Info($"thirdArg = {thirdArg}");
#endif

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

                                            case "-thing":
                                                command.KindOfNewCommand = KindOfNewCommand.Thing;
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

#if DEBUG
                                //_logger.Info($"secondArg = {secondArg}");
#endif

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

                    default:
                        throw new ArgumentOutOfRangeException(nameof(firstArg), firstArg, null);
                }

                argsList = argsList.Skip(reallyUsedArgs).ToList();
            }

#if DEBUG
            //_logger.Info($"command = {command}");
#endif

            return command;
        }

        private const string NewPureArgs = $"It is not enough arguments. You should put at least Project name!";
        private const string NewAbsenceProjectName = "ProjectName is absent!";
        private const string TimeoutAbsenceValue = "You have used -timeout but have not difined the timeout's value in milliseconds!";

        private static bool IsAdditionalOptions(string arg)
        {
            switch(arg)
            {
                case "-nologo":
                case "-timeout":
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
                case "-thing":
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
