using NLog;
using SymOntoClay.CoreHelper;
using System;
using System.IO;

namespace SymOntoClay.CLI
{
    class Program
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            EVPath.RegVar("APPDIR", Directory.GetCurrentDirectory());

            using var app = new CLIApp();
            app.Run(args);
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            _logger.Error($"e.ExceptionObject = {e.ExceptionObject}");
        }
    }
}
