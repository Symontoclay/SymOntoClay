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

            var state = State.Init;

            CommandLineArgumentOptions currentCommandLineArgumentOptions = null;
            var currentArgumentName = string.Empty;
            var currentRawResultList = new List<object>();

            var rawResultDict = new Dictionary<string, List<object>>();

            var argsList = new Queue<string> args.ToList();


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
    }
}
