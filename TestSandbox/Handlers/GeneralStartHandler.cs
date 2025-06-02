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
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.UnityAsset.Core;
using SymOntoClay.DefaultCLIEnvironment;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using TestSandbox.CoreHostListener;
using TestSandbox.Helpers;
using TestSandbox.PlatformImplementations;
using SymOntoClay.BaseTestLib.HostListeners;
using System.Runtime;
using SymOntoClay.BaseTestLib;
using SymOntoClay.Monitor.Common.Data;
using SymOntoClay.Monitor.LogFileBuilder.TextRowOptionItems;
using SymOntoClay.Monitor.LogFileBuilder;
using SymOntoClay.Core;
using SymOntoClay.Threading;

namespace TestSandbox.Handlers
{
    public class GeneralStartHandler: BaseGeneralStartHandler
    {
#if DEBUG
        private static readonly NLog.ILogger _globalLogger = NLog.LogManager.GetCurrentClassLogger();
#endif

        public GeneralStartHandler()
            : base(new UnityTestEngineContextFactorySettings()
            {
                UseDefaultNLPSettings = true,
                ThreadingSettings = new ThreadingSettings
                {
                    AsyncEvents = new CustomThreadPoolSettings
                    {
                        MaxThreadsCount = 100,
                        MinThreadsCount = 50
                    },
                    CodeExecution = new CustomThreadPoolSettings
                    {
                        MaxThreadsCount = 100,
                        MinThreadsCount = 50
                    }
                }
            })
        {
        }

        public void Run()
        {
            _logger.Info("FF819764-4617-46ED-9326-EADFE6B1A62D", "Begin");

            //var platformListener = new TstPlatformHostListener();
            //var platformListener = new HostMethods_Tests_HostListener();
            //var platformListener = new FullGeneralized_Tests_HostListener();
            var platformListener = new TstBattleRoyaleHostListener();
            //var platformListener = new TstPlatformHostListenerWithDefaultValues();

            var factorySettings = new UnityTestEngineContextFactorySettings();
            factorySettings.HostListener = platformListener;

            factorySettings.Categories = new List<string>() { "elf" };
            factorySettings.EnableCategories = true;

            CreateMainNPC(factorySettings);

            var monitor = _world.Monitor;

            _world.Start();

            var sessionDirectoryFullName = monitor.SessionDirectoryFullName;

            _globalLogger.Info($"sessionDirectoryFullName = {sessionDirectoryFullName}");

            _globalLogger.Info($"_npc.Id = {_npc.Id}");

            var sourceDirectoryName = Path.Combine(sessionDirectoryFullName, _npc.Id);

            _globalLogger.Info($"sourceDirectoryName = {sourceDirectoryName}");

            Thread.Sleep(1000);

            _logger.Info("E90B79FF-F642-4FC1-94A6-EE3F9BBD6DD0", "|||||||||||||");

            _npc.Logger.Output("264C27BF-FC6F-4002-A957-6B59C2B41EE7", "|||||||||||||");

            Thread.Sleep(500);

            var factStr = "{: $x = {: act(M16, shoot) :} & hear(I, $x) & distance(I, $x, 15.588457107543945) & direction($x, 12) & point($x, #@[15.588457107543945, 12]) :}";
            _npc.EngineContext.Storage.InsertListenedFact(_npc.Logger, factStr);

            var factId = _npc.InsertFact(_npc.Logger, "{: see(I, #a) :}");

            _npc.Logger.Output("DEEBC565-B66B-48F5-9DF6-4716C2E1623E", "|-|-|-|-|-|-|-|-|-|-|-|-|");
            _logger.Info("ED0C4551-A685-4CC0-ADB8-ACAA419FA244", "|-|-|-|-|-|-|-|-|-|-|-|-|");

            Thread.Sleep(5000);

            _logger.Info("40669EA9-0F77-4447-B128-5E940A3DCE2D", "End");

            Thread.Sleep(500);

            var logsOutputDirectory = Path.Combine(Directory.GetCurrentDirectory(), "logs");

            _globalLogger.Info($"logsOutputDirectory = {logsOutputDirectory}");

            var options = LogFileCreatorOptions.DefaultOptions;

            options.Write(new LogFileCreatorOptions()
            {
                SourceDirectoryName = sourceDirectoryName,
                OutputDirectory = logsOutputDirectory,
                DotAppPath = @"%USERPROFILE%\Downloads\Graphviz\bin\dot.exe",
                ToHtml = true
            });

            //_globalLogger.Info($"options = {options}");

            LogFileCreator.Run(options, _globalLogger);

            _globalLogger.Info("End");
        }
    }
}
