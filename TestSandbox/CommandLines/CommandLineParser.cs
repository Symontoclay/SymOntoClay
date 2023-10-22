using Newtonsoft.Json;
using NLog;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestSandbox.CommandLines
{
    public class CommandLineParser
    {
        //private enum State
        //{
        //    Init,
        //    GotArgumentName,
        //    GotArgumentValue
        //}

#if DEBUG
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
#endif

        public void RegisterArgument(CommandLineArgumentOptions argumentOptions)
        {
#if DEBUG
            _logger.Info($"argumentOptions = {argumentOptions}");
#endif

            foreach(var name in argumentOptions.Names)
            {
#if DEBUG
                _logger.Info($"name = {name}");
#endif

                _argumentOptionsDict[name] = argumentOptions;
            }
        }

        private Dictionary<string, CommandLineArgumentOptions> _argumentOptionsDict = new Dictionary<string, CommandLineArgumentOptions>();

        public Dictionary<string, object> Parse(string[] args)
        {
#if DEBUG
            _logger.Info($"args = {args.WritePODListToString()}");
#endif

            //var state = State.Init;

            CommandLineArgumentOptions currentCommandLineArgumentOptions = null;
            var currentArgumentName = string.Empty;
            var currentRawResultList = new List<object>();

            var rawResultDict = new Dictionary<string, List<object>>();

            var argsList = new Queue<string>(args);

            var isFirstIteration = true;

            while(argsList.Any())
            {
                var arg = argsList.Dequeue();

#if DEBUG
                _logger.Info($"arg = '{arg}'");
                _logger.Info($"isFirstIteration = {isFirstIteration}");
#endif

                if (isFirstIteration)
                {
                    if (_argumentOptionsDict.ContainsKey(arg))
                    {
                        InitCurrentArgument(arg, ref currentCommandLineArgumentOptions, ref currentArgumentName, ref currentRawResultList, ref rawResultDict);

#if DEBUG
                        _logger.Info($"currentCommandLineArgumentOptions = {currentCommandLineArgumentOptions}");
                        _logger.Info($"currentArgumentName = '{currentArgumentName}'");
                        _logger.Info($"currentRawResultList = {currentRawResultList?.WritePODListToString()}");
#endif

                        TryReadValues(argsList, currentCommandLineArgumentOptions, currentRawResultList);
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }

                    isFirstIteration = false;
                }
                else
                {
                    InitCurrentArgument(arg, ref currentCommandLineArgumentOptions, ref currentArgumentName, ref currentRawResultList, ref rawResultDict);

#if DEBUG
                    _logger.Info($"currentCommandLineArgumentOptions = {currentCommandLineArgumentOptions}");
                    _logger.Info($"currentArgumentName = '{currentArgumentName}'");
                    _logger.Info($"currentRawResultList = {currentRawResultList?.WritePODListToString()}");
#endif

                    TryReadValues(argsList, currentCommandLineArgumentOptions, currentRawResultList);
                }
            }

#if DEBUG
            _logger.Info($"rawResultDict = {JsonConvert.SerializeObject(rawResultDict, Formatting.Indented)}");
#endif


            //            foreach (string arg in args)
            //            {
            //#if DEBUG
            //                _logger.Info($"arg = '{arg}'");
            //                _logger.Info($"state = {state}");
            //                _logger.Info($"currentCommandLineArgumentOptions = {currentCommandLineArgumentOptions}");
            //                _logger.Info($"currentArgumentName = '{currentArgumentName}'");
            //                _logger.Info($"currentRawResultList = {currentRawResultList?.WritePODListToString()}");
            //#endif

            //                switch (state)
            //                {
            //                    case State.Init:
            //                        {
            //                            if(_argumentOptionsDict.ContainsKey(arg))
            //                            {
            //                                currentCommandLineArgumentOptions = _argumentOptionsDict[arg];
            //                                currentArgumentName = currentCommandLineArgumentOptions.Name;

            //                                if(rawResultDict.ContainsKey(currentArgumentName))
            //                                {
            //                                    currentRawResultList = rawResultDict[currentArgumentName];
            //                                }
            //                                else
            //                                {
            //                                    currentRawResultList = new List<object>();
            //                                    rawResultDict[currentArgumentName] = currentRawResultList;
            //                                }

            //                                state = State.GotArgumentName;
            //                                break;
            //                            }
            //                            else
            //                            {
            //                                throw new NotImplementedException();
            //                            }

            //                            throw new NotImplementedException();
            //                        }

            //                    case State.GotArgumentName:
            //                        {
            //                            throw new NotImplementedException();
            //                        }

            //                    default:
            //                        throw new ArgumentOutOfRangeException(nameof(state), state, null);
            //                }
            //            }

            //throw new NotImplementedException();
            return new Dictionary<string, object>();
        }

        private void InitCurrentArgument(string arg, ref CommandLineArgumentOptions currentCommandLineArgumentOptions, ref string currentArgumentName, 
            ref List<object> currentRawResultList, ref Dictionary<string, List<object>> rawResultDict)
        {
            currentCommandLineArgumentOptions = _argumentOptionsDict[arg];
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

        private void TryReadValues(Queue<string> argsList, CommandLineArgumentOptions currentCommandLineArgumentOptions, List<object> currentRawResultList)
        {
#if DEBUG
            _logger.Info($"currentCommandLineArgumentOptions = {currentCommandLineArgumentOptions}");
#endif

            while (argsList.Any())
            {
                var arg = argsList.Peek();

#if DEBUG
                _logger.Info($"arg = '{arg}'");
#endif

                if(_argumentOptionsDict.ContainsKey(arg))
                {
                    return;
                }

                argsList.Dequeue();

#if DEBUG
                _logger.Info($"arg NEXT = '{arg}'");
#endif

                var currentCommandLineArgumentOptionsKind = currentCommandLineArgumentOptions.Kind;

#if DEBUG
                _logger.Info($"currentCommandLineArgumentOptionsKind = {currentCommandLineArgumentOptionsKind}");
#endif

                switch(currentCommandLineArgumentOptionsKind)
                {
                    case KindOfCommandLineArgument.Flag:
                        throw new NotImplementedException();

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
