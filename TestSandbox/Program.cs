using NLog;
using System;
using System.Threading;
using TestSandbox.Handlers;

namespace TestSandbox
{
    class Program
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            TstGeneralStartHandler();

            //Thread.Sleep(10000);
        }

        private static void TstGeneralStartHandler()
        {
            _logger.Info("Begin");

            var handler = new GeneralStartHandler();
            handler.Run();

            _logger.Info("End");
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            _logger.Info($"e.ExceptionObject = {e.ExceptionObject}");
        }
    }
}
