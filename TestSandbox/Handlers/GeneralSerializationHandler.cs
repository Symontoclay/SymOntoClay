using SymOntoClay.BaseTestLib;
using SymOntoClay.Core;
using SymOntoClay.Monitor.LogFileBuilder;
using SymOntoClay.Threading;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using TestSandbox.CoreHostListener;

namespace TestSandbox.Handlers
{
    public class GeneralSerializationHandler : BaseGeneralStartHandler
    {
#if DEBUG
        private static readonly NLog.ILogger _globalLogger = NLog.LogManager.GetCurrentClassLogger();
#endif

        public GeneralSerializationHandler()
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
            _logger.Info("208500C0-DC47-42FF-9CCF-5E177DC33E6A", "Begin");

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

            Thread.Sleep(100);

            _logger.Info("7110252E-35E1-474E-86A6-8AF242E3053A", "|||||||||||||");

            _npc.Logger.Output("4A66829D-A6BF-4F1C-8A7E-83C8A5D1C81C", "|||||||||||||");

            Thread.Sleep(500);

            var factStr = "{: $x = {: act(M16, shoot) :} & hear(I, $x) & distance(I, $x, 15.588457107543945) & direction($x, 12) & point($x, #@[15.588457107543945, 12]) :}";
            _npc.EngineContext.Storage.InsertListenedFact(_npc.Logger, factStr);

            var factId = _npc.InsertFact(_npc.Logger, "{: see(I, #a) :}");

            _npc.Logger.Output("E4DBB6B2-5D5F-4C26-B5C7-2533E2EEA343", "|-|-|-|-|-|-|-|-|-|-|-|-|");
            _logger.Info("7B14ED65-D239-4C3D-B31E-571E1FA8E105", "|-|-|-|-|-|-|-|-|-|-|-|-|");

            Thread.Sleep(5000);

            _logger.Info("5FEAB1E4-3A21-48AE-80DF-F7BB827B25FC", "End");

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
