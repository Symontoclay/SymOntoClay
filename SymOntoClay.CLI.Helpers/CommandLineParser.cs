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
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.CLI.Helpers
{
    public class CommandLineParser
    {
#if DEBUG
        //private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
#endif

        public void RegisterArgument(CommandLineArgumentOptions argumentOptions)
        {
#if DEBUG
            //_logger.Info($"argumentOptions = {argumentOptions}");
#endif

            if(string.IsNullOrWhiteSpace(argumentOptions.Name))
            {
                throw new Exception($"Name of option cannot be null or empty.");
            }

            _argumentOptionsList.Add(argumentOptions);

            foreach (var name in argumentOptions.Names)
            {
#if DEBUG
                //_logger.Info($"name = {name}");
#endif

                _argumentOptionsDict[name] = argumentOptions;
            }
        }

        private Dictionary<string, CommandLineArgumentOptions> _argumentOptionsDict = new Dictionary<string, CommandLineArgumentOptions>();
        private List<CommandLineArgumentOptions> _argumentOptionsList = new List<CommandLineArgumentOptions>();

        public Dictionary<string, object> Parse(string[] args)
        {
#if DEBUG
            //_logger.Info($"args = {args.WritePODListToString()}");
#endif

            var defaultCommandLineArgumentOptionsList = _argumentOptionsList.Where(p => p.IsDefault).ToList();

#if DEBUG
            //_logger.Info($"defaultCommandLineArgumentOptionsList = {JsonConvert.SerializeObject(defaultCommandLineArgumentOptionsList, Formatting.Indented)}");
#endif

            if (defaultCommandLineArgumentOptionsList.Count > 1)
            {
                throw new Exception($"Too many options set as default: {string.Join(", ", defaultCommandLineArgumentOptionsList.Select(p => $"'{p.Name}'"))}. There can be only one default option.");
            }

            var defaultCommandLineArgumentOptions = defaultCommandLineArgumentOptionsList.SingleOrDefault();

#if DEBUG
            //_logger.Info($"defaultCommandLineArgumentOptions = {JsonConvert.SerializeObject(defaultCommandLineArgumentOptions, Formatting.Indented)}");
#endif

            CommandLineArgumentOptions currentCommandLineArgumentOptions = null;
            var currentArgumentName = string.Empty;
            var currentRawResultList = new List<object>();

            var rawResultDict = new Dictionary<string, List<object>>();

            var argsList = new Queue<string>(args);

            var isFirstIteration = true;

            while (argsList.Any())
            {
                var arg = argsList.Peek();

#if DEBUG
                //_logger.Info($"arg = '{arg}'");
                //_logger.Info($"isFirstIteration = {isFirstIteration}");
#endif

                if (isFirstIteration)
                {
                    if (_argumentOptionsDict.ContainsKey(arg))
                    {
                        argsList.Dequeue();

                        InitCurrentArgument(arg, ref currentCommandLineArgumentOptions, ref currentArgumentName, ref currentRawResultList, ref rawResultDict);

#if DEBUG
                        //_logger.Info($"currentCommandLineArgumentOptions = {currentCommandLineArgumentOptions}");
                        //_logger.Info($"currentArgumentName = '{currentArgumentName}'");
                        //_logger.Info($"currentRawResultList = {currentRawResultList?.WritePODListToString()}");
#endif

                        TryReadValues(arg, argsList, currentCommandLineArgumentOptions, currentRawResultList);
                    }
                    else
                    {
                        if (defaultCommandLineArgumentOptions == null)
                        {
                            throw new Exception($"There was value '{arg}' without any option. But there is not any option declared as default.");
                        }
                        else
                        {
                            currentCommandLineArgumentOptions = defaultCommandLineArgumentOptions;

                            FillUpCurrentArgumentVars(currentCommandLineArgumentOptions, ref currentArgumentName,
                                ref currentRawResultList, ref rawResultDict);

#if DEBUG
                            //_logger.Info($"currentCommandLineArgumentOptions = {currentCommandLineArgumentOptions}");
                            //_logger.Info($"currentArgumentName = '{currentArgumentName}'");
                            //_logger.Info($"currentRawResultList = {currentRawResultList?.WritePODListToString()}");
#endif

                            TryReadValues(currentArgumentName, argsList, currentCommandLineArgumentOptions, currentRawResultList);
                        }
                    }

                    isFirstIteration = false;
                }
                else
                {
                    argsList.Dequeue();

                    InitCurrentArgument(arg, ref currentCommandLineArgumentOptions, ref currentArgumentName, ref currentRawResultList, ref rawResultDict);

#if DEBUG
                    //_logger.Info($"currentCommandLineArgumentOptions = {currentCommandLineArgumentOptions}");
                    //_logger.Info($"currentArgumentName = '{currentArgumentName}'");
                    //_logger.Info($"currentRawResultList = {currentRawResultList?.WritePODListToString()}");
#endif

                    TryReadValues(arg, argsList, currentCommandLineArgumentOptions, currentRawResultList);
                }
            }

#if DEBUG
            //_logger.Info($"rawResultDict = {JsonConvert.SerializeObject(rawResultDict, Formatting.Indented)}");
#endif

            var result = new Dictionary<string, object>();

            foreach (var item in rawResultDict)
            {
                var arg = item.Key;

                var valuesList = item.Value;
#if DEBUG
                //_logger.Info($"arg = {arg}");
                //_logger.Info($"valuesList = {JsonConvert.SerializeObject(valuesList, Formatting.Indented)}");
#endif

                var commandLineArgumentOptions = _argumentOptionsDict[arg];

#if DEBUG
                //_logger.Info($"commandLineArgumentOptions = {JsonConvert.SerializeObject(commandLineArgumentOptions, Formatting.Indented)}");
#endif

                var commandLineArgumentOptionsKind = commandLineArgumentOptions.Kind;

#if DEBUG
                //_logger.Info($"commandLineArgumentOptionsKind = {commandLineArgumentOptionsKind}");
#endif

                switch (commandLineArgumentOptionsKind)
                {
                    case KindOfCommandLineArgument.Flag:
                        result[arg] = valuesList == null ? false : true;
                        break;

                    case KindOfCommandLineArgument.SingleValue:
                        result[arg] = valuesList.FirstOrDefault();
                        break;

                    case KindOfCommandLineArgument.List:
                    case KindOfCommandLineArgument.SingleValueOrList:
                        result[arg] = valuesList;
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(commandLineArgumentOptionsKind), commandLineArgumentOptionsKind, null);
                }
            }

            return result;
        }

        private void InitCurrentArgument(string arg, ref CommandLineArgumentOptions currentCommandLineArgumentOptions, ref string currentArgumentName,
            ref List<object> currentRawResultList, ref Dictionary<string, List<object>> rawResultDict)
        {
            currentCommandLineArgumentOptions = _argumentOptionsDict[arg];

            FillUpCurrentArgumentVars(currentCommandLineArgumentOptions, ref currentArgumentName,
            ref currentRawResultList, ref rawResultDict);
        }

        private void FillUpCurrentArgumentVars(CommandLineArgumentOptions currentCommandLineArgumentOptions, ref string currentArgumentName,
            ref List<object> currentRawResultList, ref Dictionary<string, List<object>> rawResultDict)
        {
            currentArgumentName = currentCommandLineArgumentOptions.Name;

            if (rawResultDict.ContainsKey(currentArgumentName))
            {
                currentRawResultList = rawResultDict[currentArgumentName];
            }
            else
            {
                currentRawResultList = new List<object>();
                rawResultDict[currentArgumentName] = currentRawResultList;
            }
        }

        private void TryReadValues(string currentOriginalCommandLineArgumentOptionsName, Queue<string> argsList, CommandLineArgumentOptions currentCommandLineArgumentOptions, List<object> currentRawResultList)
        {
#if DEBUG
            //_logger.Info($"currentOriginalCommandLineArgumentOptionsName = '{currentOriginalCommandLineArgumentOptionsName}'");
            //_logger.Info($"currentCommandLineArgumentOptions = {currentCommandLineArgumentOptions}");
#endif

            while (argsList.Any())
            {
                var arg = argsList.Peek();

#if DEBUG
                //_logger.Info($"arg = '{arg}'");
#endif

                if (_argumentOptionsDict.ContainsKey(arg))
                {
                    return;
                }

                argsList.Dequeue();

#if DEBUG
                //_logger.Info($"arg NEXT = '{arg}'");
#endif

                var currentCommandLineArgumentOptionsKind = currentCommandLineArgumentOptions.Kind;

#if DEBUG
                //_logger.Info($"currentCommandLineArgumentOptionsKind = {currentCommandLineArgumentOptionsKind}");
#endif

                switch (currentCommandLineArgumentOptionsKind)
                {
                    case KindOfCommandLineArgument.Flag:
                        throw new Exception($"Option '{currentOriginalCommandLineArgumentOptionsName}' is a flag. It cannot have an argument '{arg}'.");

                    case KindOfCommandLineArgument.List:
                    case KindOfCommandLineArgument.SingleValueOrList:
                        currentRawResultList.Add(arg);
                        break;

                    case KindOfCommandLineArgument.SingleValue:
                        currentRawResultList.Clear();
                        currentRawResultList.Add(arg);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(currentCommandLineArgumentOptionsKind), currentCommandLineArgumentOptionsKind, null);
                }
            }
        }
    }
}
