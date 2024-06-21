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

using NLog;
using SymOntoClay.CLI.Helpers;
using SymOntoClay.Common;
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

            var writeOutputToTextFileAsParallel = bool.Parse(ConfigurationManager.AppSettings["ConsoleWrapper.WriteOutputToTextFileAsParallel"] ?? "false");
            var useNLogLogger = bool.Parse(ConfigurationManager.AppSettings["ConsoleWrapper.UseNLogLogger"] ?? "false");

#if DEBUG
            //_logger.Info($"writeOutputToTextFileAsParallel = {writeOutputToTextFileAsParallel}");
            //_logger.Info($"useNLogLogger = {useNLogLogger}");
            //_logger.Info($"args = {JsonConvert.SerializeObject(args, Formatting.Indented)}");
#endif

            if (useNLogLogger)
            {
                ConsoleWrapper.SetNLogLogger(_logger);
            }

            if (writeOutputToTextFileAsParallel)
            {
                ConsoleWrapper.WriteOutputToTextFileAsParallel = true;
            }

            var defaultConfig = ConfigurationManager.AppSettings["defaultConfiguration"];

#if DEBUG
            //_logger.Info($"defaultConfig = {defaultConfig}");
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