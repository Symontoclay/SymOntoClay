using NLog;
using SymOntoClay.CLI.Helpers;
using SymOntoClay.CoreHelper;
using System;
using System.Configuration;
using System.IO;

namespace SymOntoClay.Monitor.LogFileBuilder
{
    public class Program
    {
        private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        public static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            EVPath.RegVar("APPDIR", Directory.GetCurrentDirectory());

#if DEBUG
            ConsoleWrapper.WriteOutputToTextFileAsParallel = true;
#endif

            var defaultConfig = ConfigurationManager.AppSettings["defaultConfiguration"];

#if DEBUG
            _logger.Info($"defaultConfig = {defaultConfig}");
#endif

            var app = new LogFileBuilderApp();
            app.Run(args, defaultConfig);
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            _logger.Info($"e.ExceptionObject = {e.ExceptionObject}");
        }
    }
}