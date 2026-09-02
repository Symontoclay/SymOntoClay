using SymOntoClay.BaseTestLib;
using SymOntoClay.CoreHelper.SerializationToImage;
using System;
using System.IO;
using TestSandbox.CoreHostListener;

namespace TestSandbox.Handlers
{
    public class GeneralStartDeserializationHandler : BaseGeneralStartHandler
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

        public GeneralStartDeserializationHandler()
            : base(CreateUnityTestEngineContextFactorySettings())
        { 
        }

        public void Run()
        {
            _logger.Info("82F4AE1F-9F9E-41B9-88EE-E07AEDD28191", "Begin");

            //var platformListener = new TstPlatformHostListener();
            //var platformListener = new HostMethods_Tests_HostListener();
            //var platformListener = new FullGeneralized_Tests_HostListener();
            var platformListener = new TstBattleRoyaleHostListener();
            //var platformListener = new TstPlatformHostListenerWithDefaultValues();
            //var platformListener = new VeryShortMethod_HostListener();
            //var platformListener = new Exec_Tests_HostListener2();

            var factorySettings = new UnityTestEngineContextFactorySettings();
            factorySettings.HostListener = platformListener;

            CreateMainNPC(factorySettings);

            var serializationPath = Path.Combine(Directory.GetCurrentDirectory(), "SerializationToImage", "Img_20260902_100634.pckg");

            _globalLogger.Info($"serializationPath = {serializationPath}");

            var baseTempPath = Path.Combine(Directory.GetCurrentDirectory(), "Temp");

            if (!Directory.Exists(baseTempPath))
            {
                Directory.CreateDirectory(baseTempPath);
            }

            var serializationSettings = new SerializationToImageSettings();
            serializationSettings.ImageFileName = serializationPath;
            serializationSettings.BaseTempPath = baseTempPath;

            _globalLogger.Info($"serializationSettings = {serializationSettings}");

            _world.LoadFromImage(serializationSettings);

            _logger.Info("5958BBB2-ED42-4698-95E3-AEEFBAA53F15", "End");
        }
    }
}
