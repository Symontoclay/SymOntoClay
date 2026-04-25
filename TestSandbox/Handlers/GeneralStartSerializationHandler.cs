using SymOntoClay.BaseTestLib;
using SymOntoClay.CoreHelper.SerializationToImage;
using SymOntoClay.Monitor.LogFileBuilder;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using TestSandbox.CoreHostListener;
using TestSandbox.Helpers;

namespace TestSandbox.Handlers
{
    public class GeneralStartSerializationHandler : BaseGeneralStartHandler
    {
#if DEBUG
        private static readonly NLog.ILogger _globalLogger = NLog.LogManager.GetCurrentClassLogger();
#endif

        private static UnityTestEngineContextFactorySettings CreateUnityTestEngineContextFactorySettings()
        {
            var factorySettings = new UnityTestEngineContextFactorySettings
            {
                UseDefaultNLPSettings = true
            };

            ThreadingSettingsHepler.ConfigureThreadingSettings(factorySettings);

            return factorySettings;
        }

        public GeneralStartSerializationHandler()
            : base(CreateUnityTestEngineContextFactorySettings())
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
            //var platformListener = new VeryShortMethod_HostListener();
            //var platformListener = new Exec_Tests_HostListener2();

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

            Thread.Sleep(10000);
            //Thread.Sleep(100000);

            var serializationImagesPath = Path.Combine(Directory.GetCurrentDirectory(), "Images");

            if(!Directory.Exists(serializationImagesPath))
            {
                Directory.CreateDirectory(serializationImagesPath);
            }

            var serializationPath = Path.Combine(serializationImagesPath, $"Img_{DateTime.Now:yyyyMMdd_HHmmss}.pckg");

            _globalLogger.Info($"serializationPath = {serializationPath}");

            var serializationSettings = new SerializationToImageSettings();
            serializationSettings.ImageFileName = serializationPath;

            _globalLogger.Info($"serializationSettings = {serializationSettings}");

            _world.SaveToImage(serializationSettings);

            _globalLogger.Info("|-|-|-|-|-|-|-|-|");

            Thread.Sleep(10000);

            _world.Dispose();

            _logger.Info("40669EA9-0F77-4447-B128-5E940A3DCE2D", "End");
            
            Thread.Sleep(500);
            
            /*var logsOutputDirectory = Path.Combine(Directory.GetCurrentDirectory(), "logs");

            _globalLogger.Info($"logsOutputDirectory = {logsOutputDirectory}");

            var options = LogFileCreatorOptions.DefaultOptions;

            options.Write(new LogFileCreatorOptions()
            {
                SourceDirectoryName = sourceDirectoryName,
                SerializationMode = TstEngineContextHelper.KindOfSerialization,
                OutputDirectory = logsOutputDirectory,
                DotAppPath = @"%USERPROFILE%\Downloads\Graphviz\bin\dot.exe",
                ToHtml = true,
                Mode = LogFileBuilderMode.StatAndFiles
            });

            _globalLogger.Info($"options = {options}");

            LogFileBuilderApp.Run(options);*/
            
            _globalLogger.Info("End");
        }
    }
}
