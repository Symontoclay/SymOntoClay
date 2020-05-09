using Newtonsoft.Json;
using NLog;
using SymOntoClay.Core.Internal.Helpers;
using System;
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

            TstParsing();
            //TstGeneralStartHandler();
            //TstGetParsedFilesInfo();

            //Thread.Sleep(10000);
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
