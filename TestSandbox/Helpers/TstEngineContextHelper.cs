using NLog;
using SymOntoClay.Core;
using SymOntoClay.Core.Internal;
using SymOntoClay.UnityAsset.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TestSandbox.PlatformImplementations;

namespace TestSandbox.Helpers
{
    public static class TstEngineContextHelper
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public static EngineContext CreateAndInitContext()
        {
            var logDir = Path.Combine(Directory.GetCurrentDirectory(), "NpcLogs");

            var worldSettings = new WorldSettings();

            worldSettings.SourceFilesDirs = new List<string>() { Path.Combine(Directory.GetCurrentDirectory(), "Source", "Modules") };

            worldSettings.ImagesRootDir = Path.Combine(Directory.GetCurrentDirectory(), "Images");

            worldSettings.TmpDir = Path.Combine(Directory.GetCurrentDirectory(), "Tmp");

            worldSettings.HostFile = Path.Combine(Directory.GetCurrentDirectory(), @"Source\World\World.txt");

            worldSettings.Logging = new LoggingSettings()
            {
                LogDir = logDir,
                RootContractName = "Hi1",
                PlatformLoggers = new List<IPlatformLogger>() { ConsoleLogger.Instance },
                Enable = true,
                EnableRemoteConnection = true
            };

            _logger.Info($"worldSettings = {worldSettings}");

            var npcSettings = new BipedNPCSettings();
            npcSettings.Id = "#020ED339-6313-459A-900D-92F809CEBDC5";
            npcSettings.HostFile = Path.Combine(Directory.GetCurrentDirectory(), @"Source\Hosts\PixKeeper\PixKeeper.txt");
            npcSettings.LogicFile = Path.Combine(Directory.GetCurrentDirectory(), @"Source\Apps\PixKeeper\PixKeeper.txt");

            _logger.Info($"npcSettings = {npcSettings}");

            var tmpDir = Path.Combine(worldSettings.TmpDir, npcSettings.Id);

            Directory.CreateDirectory(worldSettings.TmpDir);

            var standaloneStorageSettings = new StandaloneStorageSettings();
            standaloneStorageSettings.Id = npcSettings.Id;
            standaloneStorageSettings.IsWorld = false;
            standaloneStorageSettings.AppFile = npcSettings.HostFile;
            standaloneStorageSettings.Logger = Logger;
            standaloneStorageSettings.Dictionary = worldContext.SharedDictionary;
            standaloneStorageSettings.ModulesStorage = worldContext.ModulesStorage;
            standaloneStorageSettings.ParentStorage = worldContext.StandaloneStorage;

#if DEBUG
            _logger.Info($"standaloneStorageSettings = {standaloneStorageSettings}");
#endif
            var _hostStorage = new StandaloneStorage(standaloneStorageSettings);

            var coreEngineSettings = new EngineSettings();
            coreEngineSettings.Id = npcSettings.Id;
            coreEngineSettings.AppFile = npcSettings.LogicFile;
            coreEngineSettings.Logger = Logger;
            coreEngineSettings.SyncContext = worldContext.SyncContext;
            coreEngineSettings.Dictionary = worldContext.SharedDictionary;
            coreEngineSettings.ModulesStorage = worldContext.ModulesStorage;
            coreEngineSettings.ParentStorage = _hostStorage;
            coreEngineSettings.TmpDir = tmpDir;

#if DEBUG
            _logger.Info($"coreEngineSettings = {coreEngineSettings}");
#endif

            throw new NotImplementedException();
        }
    }
}
