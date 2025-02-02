using SymOntoClay.ActiveObject.Threads;
using SymOntoClay.BaseTestLib;
using SymOntoClay.Core;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.TasksExecution;
using SymOntoClay.Monitor.LogFileBuilder;
using SymOntoClay.Threading;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using TestSandbox.CoreHostListener;

namespace TestSandbox.Handlers
{
    public class TasksHandler: BaseGeneralStartHandler
    {
#if DEBUG
        private static readonly NLog.ILogger _globalLogger = NLog.LogManager.GetCurrentClassLogger();
#endif

        public TasksHandler()
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
            _logger.Info("2A7B9BDA-93B8-4AB3-9225-2ECC3425AEAD", "Begin");

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
            _logger.Info("EC6E70E3-294E-4492-8B7D-2F137D36628C", "-------------------------");

            var tasksPlanner = new TasksPlanner(_npc.EngineContext);

            var plan = tasksPlanner.BuildPlan();

            //_logger.Info("B7831523-8D1C-4BAB-8348-460656F67E90", $"plan = {plan}");
            _logger.Info("A74AD3BB-4D8E-44EC-AEF0-820592069E87", $"plan = {plan.ToDbgString()}");

            var context = _npc.EngineContext;

            var compiledFunctionBody = context.Compiler.Compile(plan);

            _logger.Info("F71873F5-CBD9-46E2-8B8A-304303268462", $"compiledFunctionBody = {compiledFunctionBody.ToDbgString()}");

            //var mainEntity = context.InstancesStorage.MainEntity;

            //var processInitialInfo = new ProcessInitialInfo();
            //processInitialInfo.CompiledFunctionBody = compiledFunctionBody;
            //processInitialInfo.LocalContext = mainEntity.LocalCodeExecutionContext;
            //processInitialInfo.Metadata = mainEntity.CodeItem;
            //processInitialInfo.Instance = mainEntity;
            //processInitialInfo.ExecutionCoordinator = mainEntity.ExecutionCoordinator;

            //var task = context.CodeExecutor.ExecuteAsync(context.Logger, processInitialInfo);

            //var tasksPlanRunner = new TasksPlanRunner(_npc.EngineContext, new SyncActivePeriodicObject(_npc.EngineContext.GetCancellationToken()));

            //tasksPlanRunner.Run(plan);

            _logger.Info("6FDE9DEF-C0F2-4500-BAD5-6F3FD56A3EF9", "|-|-|-|-|-|-|-|-|-|-|-|-|");

            Thread.Sleep(10000);

            _logger.Info("6FDE9DEF-C0F2-4500-BAD5-6F3FD56A3EF9", "|---|---|---|---|---|---|");

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
