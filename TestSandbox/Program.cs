using Newtonsoft.Json;
using NLog;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using TestSandbox.Handlers;
using TestSandbox.Parsing;

namespace TestSandbox
{
    class Program
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            //TstCreateName();
            //TstParsing();
            TstGeneralStartHandler();
            //TstGetParsedFilesInfo();

            //Thread.Sleep(10000);
        }

        private static void TstCreateName()
        {
            _logger.Info("Begin");

            var parserContext = new TstParserContext();

            var nameVal1 = "dog";

            _logger.Info($"{nameof(nameVal1)} = {nameVal1}");



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

            var name = new Name(nameVal1, new List<string>(), parserContext);

            _logger.Info($"name = {name}");

            _logger.Info("Begin");
        }

        private class NormalizedNameValues
        {
            public string Name { get; set; }
            public List<string> Namespaces { get; set; }
        }

        private static NormalizedNameValues ParseName(string text)
        {
#if DEBUG
            _logger.Info($"test = {text}");
#endif

            var result = new NormalizedNameValues();

            if (!text.Contains("::") && !text.Contains("("))
            {
                result.Name = text.Trim();
                return result;
            }

            throw new NotImplementedException();
        }

        private static void TstParsing()
        {
            _logger.Info("Begin");

            var handler = new ParsingHandler();
            handler.Run();

            _logger.Info("End");
        }

        private static void TstGeneralStartHandler()
        {
            _logger.Info("Begin");

            var handler = new GeneralStartHandler();
            handler.Run();

            _logger.Info("End");
        }

        private static void TstGetParsedFilesInfo()
        {
            _logger.Info("Begin");

            var fileName = Path.Combine(Directory.GetCurrentDirectory(), @"Source\Apps\PixKeeper\PixKeeper.txt");

            _logger.Info($"fileName = {fileName}");

            var id = "#020ED339-6313-459A-900D-92F809CEBDC5";

            var resultList = FileHelper.GetParsedFilesInfo(fileName, id);

            _logger.Info($"resultList = {JsonConvert.SerializeObject(resultList, Formatting.Indented)}");

            _logger.Info("End");
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            _logger.Info($"e.ExceptionObject = {e.ExceptionObject}");
        }
    }
}
