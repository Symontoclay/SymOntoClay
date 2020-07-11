using Newtonsoft.Json;
using NLog;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Core.Internal.Helpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using TestSandbox.CodeExecution;
using TestSandbox.Handlers;
using TestSandbox.Helpers;
using TestSandbox.Parsing;
using TestSandbox.PlatformImplementations;
using TestSandbox.Threads;

namespace TestSandbox
{
    class Program
    {
        private static readonly IEntityLogger _logger = new LoggerImpementation();

        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            TstActivateMainEntity();
            //TstCompileInlineTrigger();
            //TstRegOperatorsHandler();
            //TstCreateEngineContext();
            //TstAsyncActivePeriodicObjectHandler();
            //TstSyncActivePeriodicObjectHandler();
            //TstCodeExecution();
            //TstCreateName();
            //TstExprNodeHandler();
            //TstParsing();
            //TstGeneralStartHandler();
            //TstGetParsedFilesInfo();

            //Thread.Sleep(10000);
        }

        private static void TstActivateMainEntity()
        {
            _logger.Log("Begin");

            var handler = new ActivateMainEntityHandler();
            handler.Run();

            _logger.Log("End");
        }

        private static void TstCompileInlineTrigger()
        {
            _logger.Log("Begin");

            var handler = new CompileInlineTriggerHandler();
            handler.Run();

            _logger.Log("End");
        }

        private static void TstRegOperatorsHandler()
        {
            _logger.Log("Begin");

            var handler = new RegOperatorsHandler();
            handler.Run();

            _logger.Log("End");
        }

        private static void TstCreateEngineContext()
        {
            _logger.Log("Begin");

            var context = TstEngineContextHelper.CreateAndInitContext();

            _logger.Log("End");
        }

        private static void TstAsyncActivePeriodicObjectHandler()
        {
            _logger.Log("Begin");

            var handler = new AsyncActivePeriodicObjectHandler();
            handler.Run();

            _logger.Log("End");
        }

        private static void TstSyncActivePeriodicObjectHandler()
        {
            _logger.Log("Begin");

            var handler = new SyncActivePeriodicObjectHandler();
            handler.Run();

            _logger.Log("End");
        }

        private static void TstCodeExecution()
        {
            _logger.Log("Begin");

            var handler = new CodeExecutionHandler();
            handler.Run();

            _logger.Log("End");
        }

        private static void TstCreateName()
        {
            _logger.Log("Begin");

            var parserContext = new TstParserContext();

            var nameVal1 = "dog";

            _logger.Log($"{nameof(nameVal1)} = {nameVal1}");



            //var result = ParseName(nameVal1);

            //_logger.Info($"result = {JsonConvert.SerializeObject(result, Formatting.Indented)}");

            //var nameVal2 = "dog (animal)";

            //_logger.Info($"{nameof(nameVal2)} = {nameVal2}");

            //result = ParseName(nameVal2);

            //_logger.Info($"result = {JsonConvert.SerializeObject(result, Formatting.Indented)}");

            //var nameVal3 = "dog (animal | instrument)";

            //_logger.Info($"{nameof(nameVal3)} = {nameVal3}");

            //result = ParseName(nameVal3);

            //_logger.Info($"result = {JsonConvert.SerializeObject(result, Formatting.Indented)}");

            //var nameVal4 = "dog (animal (alive))";

            //_logger.Info($"{nameof(nameVal4)} = {nameVal4}");

            //result = ParseName(nameVal4);

            //_logger.Info($"result = {JsonConvert.SerializeObject(result, Formatting.Indented)}");

            //var nameVal5 = "dog (alive::animal)";

            //_logger.Info($"{nameof(nameVal5)} = {nameVal5}");

            //result = ParseName(nameVal5);

            //_logger.Info($"result = {JsonConvert.SerializeObject(result, Formatting.Indented)}");

            //var nameVal6 = "dog (alive::animal | instrument (big))";

            //_logger.Info($"{nameof(nameVal6)} = {nameVal6}");

            //result = ParseName(nameVal6);

            //_logger.Info($"result = {JsonConvert.SerializeObject(result, Formatting.Indented)}");

            //var nameVal7 = "animal::dog";

            //result = ParseName(nameVal7);

            //_logger.Info($"result = {JsonConvert.SerializeObject(result, Formatting.Indented)}");

            //var nameVal8 = "(animal | instrument)::dog";

            //result = ParseName(nameVal8);

            //_logger.Info($"result = {JsonConvert.SerializeObject(result, Formatting.Indented)}");

            var name = NameHelper.CreateName(nameVal1, parserContext.Dictionary);

            _logger.Log($"name = {name}");

            _logger.Log("End");
        }

        private class NormalizedNameValues
        {
            public string Name { get; set; }
            public List<string> Namespaces { get; set; }
        }

        private static NormalizedNameValues ParseName(string text)
        {
#if DEBUG
            _logger.Log($"test = {text}");
#endif

            var result = new NormalizedNameValues();

            if (!text.Contains("::") && !text.Contains("("))
            {
                result.Name = text.Trim();
                return result;
            }

            throw new NotImplementedException();
        }

        private static void TstExprNodeHandler()
        {
            _logger.Log("Begin");

            var handler = new ExprNodeHandler();
            handler.Run();

            _logger.Log("End");
        }

        private static void TstParsing()
        {
            _logger.Log("Begin");

            var handler = new ParsingHandler();
            handler.Run();

            _logger.Log("End");
        }

        private static void TstGeneralStartHandler()
        {
            _logger.Log("Begin");

            var handler = new GeneralStartHandler();
            handler.Run();

            _logger.Log("End");
        }

        private static void TstGetParsedFilesInfo()
        {
            _logger.Log("Begin");

            var fileName = Path.Combine(Directory.GetCurrentDirectory(), @"Source\Apps\PixKeeper\PixKeeper.txt");

            _logger.Log($"fileName = {fileName}");

            var id = "#020ED339-6313-459A-900D-92F809CEBDC5";

            var resultList = FileHelper.GetParsedFilesInfo(fileName, id);

            _logger.Log($"resultList = {JsonConvert.SerializeObject(resultList, Formatting.Indented)}");

            _logger.Log("End");
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            _logger.Log($"e.ExceptionObject = {e.ExceptionObject}");
        }
    }
}
