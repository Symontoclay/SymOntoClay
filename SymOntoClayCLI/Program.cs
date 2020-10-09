using NLog;
using System;

namespace SymOntoClay.CLI
{
    class Program
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            _logger.Error($"e.ExceptionObject = {e.ExceptionObject}");
        }
    }
}
